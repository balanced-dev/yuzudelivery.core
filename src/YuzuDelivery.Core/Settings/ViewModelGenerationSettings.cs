using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using Fluid;

namespace YuzuDelivery.Core.Settings;

public class ViewModelGenerationSettings
{
    public const bool StaticIsActive = true;
    public const string StaticDirectory = "./Yuzu/ViewModels";
    public const string DefaultGeneratedViewModelsNamespace = "YuzuDelivery.ViewModels";

    [DefaultValue(StaticIsActive)]
    public bool IsActive { get; set; } = StaticIsActive;

    [DefaultValue(false)]
    public bool AcceptUnsafeDirectory { get; set; }

    [DefaultValue(StaticDirectory)]
    public string Directory { get; set; } = StaticDirectory;

    [DefaultValue(DefaultGeneratedViewModelsNamespace)]
    public string GeneratedViewModelsNamespace { get; set; } = DefaultGeneratedViewModelsNamespace;
    public IList<string> AddNamespacesAtGeneration { get; } = new List<string> {"YuzuDelivery.Core"};
    public IList<string> ExcludeViewModelsAtGeneration { get; } = new List<string>();
    public IDictionary<string, string> ClassLevelAttributeTemplates { get; } = new Dictionary<string, string>();
    public IList<KeyValuePair<string, FilterDelegate>> CustomFilters { get; } = new List<KeyValuePair<string, FilterDelegate>>();
    public IList<Assembly> TemplateAssemblies { get; } = new List<Assembly>();

    public IDictionary<string, string> OverrideYuzuAutoMapAttributeValue { get; } = new Dictionary<string, string>();
}
