using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using NJsonSchema.CodeGeneration;
using NJsonSchema.CodeGeneration.CSharp;
using NJsonSchema.CodeGeneration.CSharp.Models;

namespace YuzuDelivery.Core.ViewmodelBuilder.NJsonSchema.CodeGeneration;

public class YuzuCSharpGenerator : CSharpGenerator
{
    public YuzuCSharpGenerator(object rootObject, CSharpGeneratorSettings settings)
        : base(rootObject, settings)
    { }

    protected override string GenerateFile(IEnumerable<CodeArtifact> artifactCollection)
    {
        var model = new FileTemplateModel()
        {
            Namespace = Settings.Namespace ?? string.Empty,
            TypesCode = artifactCollection.Concatenate(),
        };

        var template = Settings.TemplateFactory.CreateTemplate("CSharp", "File", model);
        var output = template.Render();

        var ast = CSharpSyntaxTree.ParseText(output);
        return ast.GetRoot().NormalizeWhitespace().ToFullString();
    }
}
