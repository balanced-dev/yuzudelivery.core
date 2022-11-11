
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Fluid;

namespace YuzuDelivery.Core.ViewModelBuilder
{
    public interface IYuzuViewmodelsBuilderConfig
    {
        bool EnableViewmodelsBuilder { get; set; }
        string GeneratedViewmodelsNamespace { get; set; }
        List<string> ExcludeViewmodelsAtGeneration { get; set; }
        List<string> AddNamespacesAtGeneration { get; set; }
        string GeneratedViewmodelsOutputFolder { get; set; }
        public Dictionary<string, string> ClassLevelAttributeTemplates { get; set; }
        public List<KeyValuePair<string,FilterDelegate>> CustomFilters { get; set; }
        public List<Assembly> TemplateAssemblies { get; set; }
    }

    public interface IUpdateableVmBuilderConfig
    {
        List<string> ExcludeViewmodelsAtGeneration { get; set; }
        List<string> AddNamespacesAtGeneration { get; set; }
        public Dictionary<string, string> ClassLevelAttributeTemplates { get; set; }
        public List<KeyValuePair<string,FilterDelegate>> CustomFilters { get; set; }
        public List<Assembly> TemplateAssemblies { get; set; }
    }
}
