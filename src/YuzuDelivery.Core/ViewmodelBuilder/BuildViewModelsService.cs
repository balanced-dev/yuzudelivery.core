using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using YuzuDelivery.Core;

namespace YuzuDelivery.Core.ViewModelBuilder
{
    public class BuildViewModelsService
    {
        private readonly GenerateViewmodelService generateViewmodelService;
        private readonly IEnumerable<IViewmodelPostProcessor> postProcessors;
        private readonly IYuzuViewmodelsBuilderConfig builderConfig;

        private string pagePath;
        private string blockPath;

        public string OutputPath { get; set; }

        public BuildViewModelsService(
            GenerateViewmodelService generateViewmodelService,
            IEnumerable<IViewmodelPostProcessor> postProcessors,
            IYuzuConfiguration config,
            IYuzuViewmodelsBuilderConfig builderConfig)
        {
            this.generateViewmodelService = generateViewmodelService;
            this.postProcessors = postProcessors;
            this.builderConfig = builderConfig;

            pagePath = config.TemplateLocations.Where(x => x.Name == "Pages").Select(x => x.Schema).FirstOrDefault();
            blockPath = config.TemplateLocations.Where(x => x.Name == "Partials").Select(x => x.Schema).FirstOrDefault();
            pagePath = pagePath.EndsWith("/") ? pagePath : string.Format("{0}\\", pagePath);
            blockPath = blockPath.EndsWith("/") ? blockPath : string.Format("{0}\\", blockPath);
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

                foreach(var p in postProcessors)
                {
                    if (p.IsValid(file.Name))
                        file.Content = p.Apply(file.Content);
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
            return generateViewmodelService.Create(string.Format(@"{0}par{1}.schema", GetDirectoryForType(viewModelType), schemaName), schemaName, viewModelType,  blockPath, builderConfig);
        }

        public virtual void WriteOutputFile(string outputFilename, string content)
        {
            File.WriteAllText(string.Format(@"{0}\{1}.cs", OutputPath, outputFilename), content);
        }

    }
}
