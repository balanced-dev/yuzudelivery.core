using System;
using System.Reflection;
using System.Collections.Generic;
using YuzuDelivery.Core.Mapping;

namespace YuzuDelivery.Core
{
    public interface IYuzuConfiguration
    { 
        List<Assembly> MappingAssemblies { get; set; }

        List<Assembly> ViewModelAssemblies { get; }
        List<Type> ViewModels { get; }
        List<Type> CMSModels { get; set; }

        List<ManualMapInstalledType> InstalledManualMaps { get; }
        List<ManualMapActiveType> ActiveManualMaps { get; }
        Dictionary<Type, Func<IYuzuTypeFactory>> ViewmodelFactories { get; }
        List<ITemplateLocation> TemplateLocations { get; set; }

        Func<IRenderSettings, string> GetRenderedHtmlCache { get; set; }
        Action<IRenderSettings, string> SetRenderedHtmlCache { get; set; }

        Func<Dictionary<string, Func<object, string>>> GetTemplatesCache { get; set; }
        Func<Dictionary<string, Func<object, string>>> SetTemplatesCache { get; set; }

        void AddActiveManualMap<Resolver, Dest>(string destPropertyName = null);
        bool HasActiveManualMap(string dest, string destMemberName = null);
    }

    public interface IUpdateableConfig
    {
        List<Assembly> MappingAssemblies { get; set; }
    }

    public interface IChildSiteConfig 
    {
        void Setup(IYuzuConfiguration _config);     
    }
}
