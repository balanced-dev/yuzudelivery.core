using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Extensions.Logging;
using NJsonSchema.CodeGeneration;
using NJsonSchema.CodeGeneration.CSharp;
using YuzuDelivery.Core.ViewmodelBuilder.NJsonSchema.CodeGeneration.Models;

namespace YuzuDelivery.Core.ViewmodelBuilder.NJsonSchema.CodeGeneration;

public class YuzuCSharpGenerator : CSharpGenerator
{
    private readonly ILogger<YuzuCSharpGenerator> _logger;

    public YuzuCSharpGenerator(ILogger<YuzuCSharpGenerator> logger, object rootObject, YuzuCSharpGeneratorSettings settings)
        : base(rootObject, settings)
    {
        _logger = logger;
    }

    protected override string GenerateFile(IEnumerable<CodeArtifact> artifactCollection)
    {
        var model = new YuzuFileTemplateModel()
        {
            Namespace = Settings.Namespace ?? string.Empty,
            TypesCode = artifactCollection.Concatenate(),
            AdditionalNamespaces = GetAdditionalNamespaces().ToList()
        };

        var template = Settings.TemplateFactory.CreateTemplate("CSharp", "File", model);
        var output = template.Render();

        var ast = CSharpSyntaxTree.ParseText(output);
        return ast.GetRoot().NormalizeWhitespace().ToFullString();
    }

    private IEnumerable<string> GetAdditionalNamespaces()
    {
        var settings = (YuzuCSharpGeneratorSettings) Settings;

        foreach (var ns in settings.AdditionalNamespaces)
        {
            var match = Regex.Match(ns, @"using\s+(.+);");
            if (match.Success)
            {
                _logger.LogWarning("Additional namespaces no longer require the full using directive: \"{setting}\"", ns);
                yield return match.Groups[1].Value;
            }
            else
            {
                yield return ns;
            }
        }
    }
}
