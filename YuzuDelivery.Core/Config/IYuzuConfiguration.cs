using System;
using System.Reflection;
using System.Collections.Generic;

namespace YuzuDelivery.Core
{
    public interface IYuzuConfiguration
    {
        IEnumerable<Type> ViewModels { get; }
        Assembly[] ViewModelAssemblies { get; set; }

        string ViewModelQualifiedTypeName { get; set; }

        string BlockPrefix { get; set; }
        string SubPrefix { get; set; }
        string PagePrefix { get; set; }
        string BlockRefPrefix { get; set; }

        string TemplateFileExtension { get; set; }
        List<IDataLocation> SchemaMetaLocations { get; set; }
        List<ITemplateLocation> TemplateLocations { get; set; }

        Func<IRenderSettings, string> GetRenderedHtmlCache { get; set; }
        Action<IRenderSettings, string> SetRenderedHtmlCache { get; set; }

        Func<Dictionary<string, Func<object, string>>> GetTemplatesCache { get; set; }
        Func<Dictionary<string, Func<object, string>>> SetTemplatesCache { get; set; }

        List<string> ExcludeViewmodelsAtGeneration { get; set; }
        List<string> AddNamespacesAtGeneration { get; set; }
    }
}