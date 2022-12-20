using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace YuzuDelivery.Core.ViewModelBuilder
{
    public class BuildViewModelsService
    {
        private readonly GenerateViewmodelService generateViewmodelService;
        private readonly IYuzuViewmodelsBuilderConfig builderConfig;

        private string pagePath;
        private string blockPath;

        public string OutputPath { get; set; }

        public BuildViewModelsService(
            GenerateViewmodelService generateViewmodelService,
            IYuzuConfiguration config,
            IYuzuViewmodelsBuilderConfig builderConfig)
        {
            this.generateViewmodelService = generateViewmodelService;
            this.builderConfig = builderConfig;

            pagePath = config.TemplateLocations.Where(x => x.TemplateType == TemplateType.Page).Select(x => x.Schema).FirstOrDefault();
            blockPath = config.TemplateLocations.Where(x => x.TemplateType == TemplateType.Partial).Select(x => x.Schema).FirstOrDefault();

            if (pagePath != null && !pagePath.EndsWith(Path.DirectorySeparatorChar))
            {
                pagePath += Path.DirectorySeparatorChar;
            }

            if (blockPath != null && !blockPath.EndsWith(Path.DirectorySeparatorChar))
            {
                blockPath += Path.DirectorySeparatorChar;
            }
        }

        public string GetDirectoryForType(ViewModelType viewModelType)
        {
            return viewModelType == ViewModelType.page ? pagePath : blockPath;
        }

        public void RunAll(ViewModelType viewModelType, Dictionary<string, string> output = null, int limit = 99999)
        {
            DirectoryInfo dir = new DirectoryInfo(GetDirectoryForType(viewModelType));
            var count = 0;

            foreach(var i in dir.GetFiles("*.schema"))
            {
                if (count == limit)
                    break;

                (string Name, string Content) file = (Name: string.Empty, Content: string.Empty);

                try
                {
                    file = generateViewmodelService.Create(i.FullName, i.Name.CleanFileExtension().RemoveFileSuffix(), viewModelType, blockPath, builderConfig);
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("Failed on schema file {0}", i.Name), ex);
                }

                if(file.Content.Contains("public partial class") || file.Content.Contains("public enum"))
                {
                    if (output != null)
                        output.Add(file.Name, file.Content);
                    else
                        WriteOutputFile(file.Name, file.Content);

                    count++;
                }
            }
        }

        public (string Name, string Content) RunOneBlock(ViewModelType viewModelType, string schemaName)
        {
            var schemaFilename = $"par{schemaName}.schema";
            return generateViewmodelService.Create(Path.Combine(GetDirectoryForType(viewModelType), schemaFilename), schemaName, viewModelType,  blockPath, builderConfig);
        }

        public virtual void WriteOutputFile(string outputFilename, string content)
        {
            var outputFilenameWithExtension = $"{outputFilename}.cs";
            File.WriteAllText(Path.Combine(OutputPath, outputFilenameWithExtension), content);
        }

    }
}
