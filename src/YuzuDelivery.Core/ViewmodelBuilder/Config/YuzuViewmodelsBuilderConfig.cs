using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Fluid;

namespace YuzuDelivery.Core.ViewModelBuilder
{
    public class YuzuViewmodelsBuilderConfig : IYuzuViewmodelsBuilderConfig, IUpdateableVmBuilderConfig
    {
        public YuzuViewmodelsBuilderConfig()
            : this(Enumerable.Empty<IUpdateableVmBuilderConfig>())
        { }

        public YuzuViewmodelsBuilderConfig(IEnumerable<IUpdateableVmBuilderConfig> extraConfig)
        {
            GeneratedViewmodelsNamespace = "YuzuDelivery.ViewModels";

            foreach (var c in extraConfig)
            {
                AddNamespacesAtGeneration = AddNamespacesAtGeneration.Union(c.AddNamespacesAtGeneration).ToList();
                ExcludeViewmodelsAtGeneration = ExcludeViewmodelsAtGeneration.Union(c.ExcludeViewmodelsAtGeneration).ToList();
                CustomFilters = CustomFilters.Union(c.CustomFilters).ToList();
                TemplateAssemblies = TemplateAssemblies.Union(c.TemplateAssemblies).ToList();

                foreach (var kvp in c.ClassLevelAttributeTemplates)
                {
                    ClassLevelAttributeTemplates[kvp.Key] = kvp.Value;
                }
            }
        }

        public bool EnableViewmodelsBuilder { get; set; }
        public string GeneratedViewmodelsNamespace { get; set; }
        public string GeneratedViewmodelsOutputFolder { get; set; }

        public List<string> AddNamespacesAtGeneration { get; set; } = new();
        public List<string> ExcludeViewmodelsAtGeneration { get; set; } = new();
        public Dictionary<string, string> ClassLevelAttributeTemplates { get; set; } = new();
        public List<KeyValuePair<string, FilterDelegate>> CustomFilters { get; set; } = new();
        public List<Assembly> TemplateAssemblies { get; set; } = new();
    }

    public abstract class UpdateableVmBuilderConfig : IUpdateableVmBuilderConfig
    {
        public List<string> AddNamespacesAtGeneration { get; set; } = new();
        public List<string> ExcludeViewmodelsAtGeneration { get; set; } = new();
        public Dictionary<string, string> ClassLevelAttributeTemplates { get; set; } = new();
        public List<KeyValuePair<string, FilterDelegate>> CustomFilters { get; set; } = new();
        public List<Assembly> TemplateAssemblies { get; set; } = new();
    }

}
