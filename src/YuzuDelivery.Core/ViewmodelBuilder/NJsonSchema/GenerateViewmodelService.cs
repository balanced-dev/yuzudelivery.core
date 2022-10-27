using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using NJsonSchema;
using NJsonSchema.CodeGeneration.CSharp;
using YuzuDelivery.Core.ViewmodelBuilder.NJsonSchema.CodeGeneration;

namespace YuzuDelivery.Core.ViewModelBuilder
{
    public class GenerateViewmodelService
    {
        public (string Name, string Content) Create(string schemaFilename, string outputFilename, ViewModelType viewModelType, List<string> excludedViewModels, string blockPath, string namespaceValue)
        {
            var file = ReadFile(schemaFilename);
            string fileOut = string.Empty;

            var excludedTypes = GetExcludedTypesFromFiles(outputFilename, blockPath);
            excludedTypes.Add("DoNotApply");
            excludedTypes = excludedTypes.Union(excludedViewModels).ToList();

            var schema = JsonSchema.FromJsonAsync(file, blockPath).Result;

            if (outputFilename != schema.Id.SchemaIdToName())
                throw new Exception(string.Format("Filename and schema id must match, {0} doesn't equal {1} and schema id must start with a /", outputFilename, schema.Id.SchemaIdToName()));

            var csharpSetting = new CSharpGeneratorSettings
            {
                Namespace = namespaceValue,
                ArrayType = "System.Collections.Generic.List",
                ArrayBaseType = "System.Collections.Generic.List",
                ArrayInstanceType = "System.Collections.Generic.List",
                ClassStyle = CSharpClassStyle.Poco,
                TypeNameGenerator = new ViewModelTypeNameGenerator(),
                PropertyNameGenerator = new ViewModelPropertyNameGenerator(),
                ExcludedTypeNames = excludedTypes.ToArray(),
            };

            csharpSetting.TemplateFactory = new YuzuTemplateFactory(csharpSetting);

            SchemaName.RootSchema = outputFilename;
            SchemaName.ViewModelType = viewModelType;

            var generator = new CSharpGenerator(schema, csharpSetting);
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
