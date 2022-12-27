using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using YuzuDelivery.Core.Settings;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;

namespace YuzuDelivery.Core.ViewModelBuilder
{
    public class BuildViewModelsService
    {
        private readonly GenerateViewmodelService generateViewmodelService;
        private readonly IYuzuViewmodelsBuilderConfig builderConfig;
        private readonly IFileProvider schemaFileProvider;

        private IList<IFileInfo> all;
        private IDictionary<string, IFileInfo> blocks;

        public string OutputPath { get; set; }

        public BuildViewModelsService(
            GenerateViewmodelService generateViewmodelService,
            IOptions<CoreSettings> coreSettings,
            IYuzuViewmodelsBuilderConfig builderConfig)
        {
            this.generateViewmodelService = generateViewmodelService;
            this.builderConfig = builderConfig;
            this.schemaFileProvider = coreSettings.Value.SchemaFileProvider;

            all = new List<IFileInfo>();
            blocks = new Dictionary<string, IFileInfo>();

            this.schemaFileProvider.GetPagesAndPartials(".schema", "par", AddFilteredFiles);
        }

        private void AddFilteredFiles(bool isPartial, string name, IFileInfo fileInfo)
        {
            if(isPartial) blocks.Add(name, fileInfo);
            all.Add(fileInfo);
        }

        public void RunAll(ViewModelType viewModelType, Dictionary<string, string> output = null, int limit = 99999)
        {
            var count = 0;

            foreach(var i in all)
            {
                if (count == limit)
                    break;

                (string Name, string Content) file = (Name: string.Empty, Content: string.Empty);

                try
                {
                    file = generateViewmodelService.Create(i, i.Name.CleanFileExtension().RemoveFileSuffix(), viewModelType, blocks, builderConfig);
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("Failed on schema file {0}", i.Name), ex);
                }

                if (file.Content.Contains("public partial class") || file.Content.Contains("public enum"))
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
            var fileInfo = all.FirstOrDefault(x => x.Name == schemaFilename);
            return generateViewmodelService.Create(fileInfo, schemaName, viewModelType, blocks, builderConfig);
        }

        public virtual void WriteOutputFile(string outputFilename, string content)
        {
            var outputFilenameWithExtension = $"{outputFilename}.cs";
            File.WriteAllText(Path.Combine(OutputPath, outputFilenameWithExtension), content);
        }

    }
}
