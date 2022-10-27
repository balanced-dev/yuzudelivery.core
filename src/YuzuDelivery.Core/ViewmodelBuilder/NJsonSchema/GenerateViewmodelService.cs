using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.Extensions.Logging;
using NJsonSchema;
using NJsonSchema.CodeGeneration.CSharp;
using YuzuDelivery.Core.ViewmodelBuilder.NJsonSchema.CodeGeneration;
using YuzuDelivery.Core.ViewmodelBuilder.NJsonSchema.CodeGeneration.Models;

namespace YuzuDelivery.Core.ViewModelBuilder
{
    public class GenerateViewmodelService
    {
        private readonly ILoggerFactory _loggerFactory;

        public GenerateViewmodelService(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
        }

        public (string Name, string Content) Create(string schemaFilename, string outputFilename, ViewModelType viewModelType,  string blockPath, IYuzuViewmodelsBuilderConfig config)
        {
            var file = ReadFile(schemaFilename);
            string fileOut = string.Empty;

            var excludedTypes = GetExcludedTypesFromFiles(outputFilename, blockPath);
            excludedTypes.Add("DoNotApply");
            excludedTypes = excludedTypes.Union(config.ExcludeViewmodelsAtGeneration).ToList();

            var schema = JsonSchema.FromJsonAsync(file, blockPath).Result;

            if (outputFilename != schema.Id.SchemaIdToName())
                throw new Exception(string.Format("Filename and schema id must match, {0} doesn't equal {1} and schema id must start with a /", outputFilename, schema.Id.SchemaIdToName()));

            var csharpSetting = new YuzuCSharpGeneratorSettings()
            {
                Namespace = config.GeneratedViewmodelsNamespace,
                ArrayType = "System.Collections.Generic.List",
                ArrayBaseType = "System.Collections.Generic.List",
                ArrayInstanceType = "System.Collections.Generic.List",
                ClassStyle = CSharpClassStyle.Poco,
                TypeNameGenerator = new ViewModelTypeNameGenerator(),
                PropertyNameGenerator = new ViewModelPropertyNameGenerator(),
                ExcludedTypeNames = excludedTypes.ToArray(),
                AdditionalNamespaces = config.AddNamespacesAtGeneration
            };

            csharpSetting.TemplateFactory = new YuzuTemplateFactory(csharpSetting);

            SchemaName.RootSchema = outputFilename;
            SchemaName.ViewModelType = viewModelType;

            var generator = new YuzuCSharpGenerator(_loggerFactory.CreateLogger<YuzuCSharpGenerator>(), schema, csharpSetting);
            fileOut = generator.GenerateFile(outputFilename);

            var actualFilename = outputFilename.AddVmTypePrefix(viewModelType);

            return (Name: actualFilename, Content: fileOut);
        }

        private string ReadFile(string schemaFilename)
        {
            return File.ReadAllText(schemaFilename);
        }

        private List<string> GetExcludedTypesFromFiles(string outputFile, string blockPath)
        {
            DirectoryInfo dir = new DirectoryInfo(blockPath);
            var excludedTypes = new List<string>();
            var currentViewModelName = outputFile.AddVmTypePrefix(ViewModelType.block);

            foreach (var i in dir.GetFiles("*.schema"))
            {
                var viewModelName = i.Name.CleanFileExtension().RemoveFileSuffix().AddVmTypePrefix(ViewModelType.block);
                if (currentViewModelName != viewModelName)
                    excludedTypes.Add(viewModelName);
            }

            return excludedTypes;
        }



    }
}
