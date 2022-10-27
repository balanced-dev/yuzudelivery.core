using System.IO;
using System.Reflection;
using NJsonSchema.CodeGeneration;
using NJsonSchema.CodeGeneration.CSharp;

namespace YuzuDelivery.Core.ViewmodelBuilder.NJsonSchema.CodeGeneration;

public class YuzuTemplateFactory : DefaultTemplateFactory
{
    public YuzuTemplateFactory(CodeGeneratorSettingsBase settings)
        : base(settings, new []{typeof(CSharpGeneratorSettings).GetTypeInfo().Assembly})
    { }

    protected override string GetEmbeddedLiquidTemplate(string language, string template)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = "YuzuDelivery.Core.ViewmodelBuilder.NJsonSchema.CodeGeneration.Templates." + template + ".liquid";
        var resource = assembly.GetManifestResourceStream(resourceName);

        // ReSharper disable once InvertIf
        if (resource != null)
        {
            using var reader = new StreamReader(resource);
            return reader.ReadToEnd();
        }

        return base.GetEmbeddedLiquidTemplate(language, template);
    }
}
