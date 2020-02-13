using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YuzuDelivery.Core;

namespace YuzuDelivery.Core.ViewModelBuilder
{
    public class FileRefViewmodelPostProcessor : IViewmodelPostProcessor
    {
        private readonly IYuzuViewmodelsBuilderConfig builderConfig;

        public FileRefViewmodelPostProcessor(IYuzuViewmodelsBuilderConfig builderConfig)
        {
            this.builderConfig = builderConfig;
        }

        public bool IsValid(string name)
        {
            return true;
        }

        public string Apply(string content)
        {
            foreach (var use in builderConfig.AddNamespacesAtGeneration.OrderByDescending(x => x))
            {
                content = string.Format("{0}\n{1}", use, content);
            }
            return content;
        }

    }
}
