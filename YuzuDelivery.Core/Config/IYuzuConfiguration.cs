using System;
using System.Reflection;
using System.Collections.Generic;

namespace YuzuDelivery.Core
{
    public interface IYuzuConfiguration
    {
        Assembly[] ViewModelAssemblies { get; set; }
        List<Assembly> MappingAssemblies { get; set; }

        IEnumerable<Type> ViewModels { get; }
        IEnumerable<Type> CMSModels { get; set; }

        List<IDataLocation> SchemaMetaLocations { get; set; }
        List<ITemplateLocation> TemplateLocations { get; set; }

        Func<IRenderSettings, string> GetRenderedHtmlCache { get; set; }
        Action<IRenderSettings, string> SetRenderedHtmlCache { get; set; }

        Func<Dictionary<string, Func<object, string>>> GetTemplatesCache { get; set; }
        Func<Dictionary<string, Func<object, string>>> SetTemplatesCache { get; set; }
    }

    public interface IUpdateableConfig
    {
        List<Assembly> MappingAssemblies { get; set; }
    }
}