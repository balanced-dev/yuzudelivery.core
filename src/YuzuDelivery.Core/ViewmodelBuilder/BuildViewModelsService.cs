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
        private readonly IOptions<ViewModelGenerationSettings> builderConfig;
        private readonly IFileProvider schemaFileProvider;

        private IDictionary<string, IFileInfo> pages;
        private IDictionary<string, IFileInfo> blocks;

        public string OutputPath { get; set; }

        public BuildViewModelsService(
            GenerateViewmodelService generateViewmodelService,
            IOptions<CoreSettings> coreSettings,
            IOptions<ViewModelGenerationSettings> builderConfig)
        {
            this.generateViewmodelService = generateViewmodelService;
            this.builderConfig = builderConfig;
            this.schemaFileProvider = coreSettings.Value.SchemaFileProvider;

            pages = new Dictionary<string, IFileInfo>();
            blocks = new Dictionary<string, IFileInfo>();

            this.schemaFileProvider.GetPagesAndPartials(coreSettings.Value.SchemaFileExtension, coreSettings.Value, AddFilteredFiles);
        }

        private void AddFilteredFiles(bool isPartial, bool isLayout, string name, IFileInfo fileInfo)
        {
            if(isPartial) blocks.Add(name, fileInfo);
            if(!isLayout && !isPartial) pages.Add(name, fileInfo);
        }

        public void RunAll(ViewModelType viewModelType, Dictionary<string, string> output = null, int limit = 99999)
        {
            var count = 0;
            var entries = viewModelType == ViewModelType.page ? pages : blocks;

            foreach(var i in entries)
            {
                if (count == limit)
                    break;

                (string Name, string Content) file = (Name: string.Empty, Content: string.Empty);

                try
                {
                    file = generateViewmodelService.Create(i.Value, i.Value.Name.CleanFileExtension().RemoveFileSuffix(), viewModelType, blocks, builderConfig.Value);
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("Failed on schema file {0}", i.Key), ex);
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
            var schemaFilename = $"par{schemaName}";
            var fileInfo = blocks.FirstOrDefault(x => x.Key == schemaFilename).Value;
            return generateViewmodelService.Create(fileInfo, schemaName, viewModelType, blocks, builderConfig.Value);
        }

        public virtual void WriteOutputFile(string outputFilename, string content)
        {
            var outputFilenameWithExtension = $"{outputFilename}.cs";
            File.WriteAllText(Path.Combine(OutputPath, outputFilenameWithExtension), content);
        }

    }
}
