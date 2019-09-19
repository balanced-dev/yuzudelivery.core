using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Reflection;

namespace YuzuDelivery.Core
{
    public class YuzuConfiguration : IYuzuConfiguration
    {
        public YuzuConfiguration()
        {
            TemplateLocations = new List<ITemplateLocation>();
            TemplateFileExtension = ".hbs";
            ExcludeViewmodelsAtGeneration = new List<string>();
            AddNamespacesAtGeneration = new List<string>();
        }

        public List<ITemplateLocation> TemplateLocations { get; set; }
        public string TemplateFileExtension { get; set; }

        public Func<Dictionary<string, Func<object, string>>> GetTemplatesCache { get; set; }
        public Func<Dictionary<string, Func<object, string>>> SetTemplatesCache { get; set; }

        public Func<IRenderSettings, string> GetRenderedHtmlCache { get; set; }
        public Action<IRenderSettings, string> SetRenderedHtmlCache { get; set; }

        public List<string> ExcludeViewmodelsAtGeneration { get; set; }
        public List<string> AddNamespacesAtGeneration { get; set; }

    }

}
