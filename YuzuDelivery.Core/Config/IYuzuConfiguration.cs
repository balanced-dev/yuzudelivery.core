using System;
using System.Collections.Generic;

namespace YuzuDelivery.Umbraco.Blocks
{
    public interface IYuzuConfiguration
    {
        string TemplateFileExtension { get; set; }
        List<ITemplateLocation> TemplateLocations { get; set; }

        Func<IRenderSettings, string> GetRenderedHtmlCache { get; set; }
        Action<IRenderSettings, string> SetRenderedHtmlCache { get; set; }

        Func<Dictionary<string, Func<object, string>>> GetTemplatesCache { get; set; }
        Func<Dictionary<string, Func<object, string>>> SetTemplatesCache { get; set; }
    }
}