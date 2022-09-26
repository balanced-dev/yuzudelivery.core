using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YuzuDelivery.Core.ViewModelBuilder
{
    public class YuzuViewmodelsBuilderConfig : IYuzuViewmodelsBuilderConfig, IUpdateableVmBuilderConfig
    {
        public YuzuViewmodelsBuilderConfig(IEnumerable<IUpdateableVmBuilderConfig> extraConfig)
        {
            GeneratedViewmodelsNamespace = "YuzuDelivery.ViewModels";
            AddNamespacesAtGeneration = new List<string>();
            ExcludeViewmodelsAtGeneration = new List<string>();

            foreach (var c in extraConfig)
            {
                AddNamespacesAtGeneration = AddNamespacesAtGeneration.Union(c.AddNamespacesAtGeneration).ToList();
                ExcludeViewmodelsAtGeneration = ExcludeViewmodelsAtGeneration.Union(c.ExcludeViewmodelsAtGeneration).ToList();
            }
        }
       
        public bool EnableViewmodelsBuilder { get; set; }
        public string GeneratedViewmodelsNamespace { get; set; }
        public string GeneratedViewmodelsOutputFolder { get; set; }

        public List<string> AddNamespacesAtGeneration { get; set; }
        public List<string> ExcludeViewmodelsAtGeneration { get; set; }

    }

    public abstract class UpdateableVmBuilderConfig : IUpdateableVmBuilderConfig
    {
        public UpdateableVmBuilderConfig()
        {
            AddNamespacesAtGeneration = new List<string>();
            ExcludeViewmodelsAtGeneration = new List<string>();
        }

        public List<string> AddNamespacesAtGeneration { get; set; }
        public List<string> ExcludeViewmodelsAtGeneration { get; set; }
    }

}
