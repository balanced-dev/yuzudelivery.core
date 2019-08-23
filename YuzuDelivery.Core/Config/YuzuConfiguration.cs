using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuzuDelivery.Umbraco.Blocks
{
    public class YuzuConfiguration : IYuzuConfiguration
    {
        public YuzuConfiguration()
        {
            TemplateFileExtension = ".hbs";
        }

        public List<ITemplateLocation> TemplateLocations { get; set; }
        public string TemplateFileExtension { get; set; }

        public Func<Dictionary<string, Func<object, string>>> GetTemplatesCache { get; set; }
        public Func<Dictionary<string, Func<object, string>>> SetTemplatesCache { get; set; }

        public Func<IRenderSettings, string> GetRenderedHtmlCache { get; set; }
        public Action<IRenderSettings, string> SetRenderedHtmlCache { get; set; }

    }
}
