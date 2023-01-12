using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using NJsonSchema;
using NJsonSchema.CodeGeneration.CSharp;
using YuzuDelivery.Core.ViewmodelBuilder.NJsonSchema.CodeGeneration;
using Microsoft.Extensions.FileProviders;
using Newtonsoft.Json.Serialization;
using System.Text.RegularExpressions;
using YuzuDelivery.Core.Settings;
using YuzuDelivery.Core.ViewmodelBuilder.NJsonSchema;

namespace YuzuDelivery.Core.ViewModelBuilder
{
    public class GenerateViewmodelService
    {
        private readonly ReferencesService _referencesService;

        public GenerateViewmodelService(ReferencesService referencesService)
        {
            _referencesService = referencesService;
        }

        public (string Name, string Content) Create(IFileInfo schemaFile, string outputFilename, ViewModelType viewModelType,  IDictionary<string, IFileInfo> blocks, ViewModelGenerationSettings config)
        {
            var fileString = ReadFile(schemaFile);
            string fileOut = string.Empty;

            var excludedTypes = GetExcludedTypesFromFiles(outputFilename, blocks);
            excludedTypes.Add("DoNotApply");
            excludedTypes = excludedTypes.Union(config.ExcludeViewModelsAtGeneration).ToList();

            var schema = JsonSchema.FromJsonAsync(fileString, ".", (JsonSchema schema) =>
            {
                var schemaResolver = new JsonSchemaAppender(schema, new DefaultTypeNameGenerator());
                return new JsonReferenceResolverForFileProvider(schemaResolver, blocks, _referencesService);
            }).Result;

            if (outputFilename != schema.Id.SchemaIdToName())
                throw new Exception(string.Format("Filename and schema id must match, {0} doesn't equal {1} and schema id must start with a /", outputFilename, schema.Id.SchemaIdToName()));

            var csharpSetting = new CSharpGeneratorSettings()
            {
                Namespace = config.GeneratedViewModelsNamespace,
                ArrayType = "System.Collections.Generic.List",
                ArrayBaseType = "System.Collections.Generic.List",
                ArrayInstanceType = "System.Collections.Generic.List",
                ClassStyle = CSharpClassStyle.Poco,
                TypeNameGenerator = new ViewModelTypeNameGenerator(),
                PropertyNameGenerator = new ViewModelPropertyNameGenerator(),
                ExcludedTypeNames = excludedTypes.ToArray(),
            };

            csharpSetting.TemplateFactory = new YuzuTemplateFactory(config);

            SchemaName.RootSchema = outputFilename;
            SchemaName.ViewModelType = viewModelType;

            var generator = new YuzuCSharpGenerator(schema, csharpSetting);
            fileOut = generator.GenerateFile(outputFilename);

            var actualFilename = outputFilename.AddVmTypePrefix(viewModelType);

            return (Name: actualFilename, Content: fileOut);
        }

        private string ReadFile(IFileInfo schemaFile)
        {
            var fileStream = schemaFile.CreateReadStream();
            using var reader = new StreamReader(fileStream);
            var json = reader.ReadToEnd();
            return _referencesService.Fix(json);
        }

        private List<string> GetExcludedTypesFromFiles(string outputFile, IDictionary<string, IFileInfo> blocks)
        {
            var excludedTypes = new List<string>();
            var currentViewModelName = outputFile.AddVmTypePrefix(ViewModelType.block);

            foreach (var i in blocks)
            {
                var viewModelName = i.Key.RemoveFileSuffix().AddVmTypePrefix(ViewModelType.block);
                if (currentViewModelName != viewModelName)
                    excludedTypes.Add(viewModelName);
            }

            return excludedTypes;
        }
    }
}
