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
            BlockPrefix = "vmBlock_";
            SubPrefix = "vmSub_";
            PagePrefix = "vmPage_";
            BlockRefPrefix = "/par";

            SchemaMetaLocations = new List<IDataLocation>();
            TemplateLocations = new List<ITemplateLocation>();
            TemplateFileExtension = ".hbs";
            ExcludeViewmodelsAtGeneration = new List<string>();
            AddNamespacesAtGeneration = new List<string>();
        }

        public virtual IEnumerable<Type> ViewModels { get; private set; }
        private Assembly[] viewModelAssemblies;
        public Assembly[] ViewModelAssemblies
        {
            get
            {
                return viewModelAssemblies;
            }
            set
            {
                viewModelAssemblies = value;
                ViewModels = value.SelectMany(x => x.GetTypes().Where(y => y.Name.IsComponentVm()));
            }
        }

        public string ViewModelQualifiedTypeName { get; set; }
        public string UmbracoModelsQualifiedTypeName { get; set; }

        public string BlockPrefix { get; set; }
        public string SubPrefix { get; set; }
        public string PagePrefix { get; set; }
        public string BlockRefPrefix { get; set; }

        public List<ITemplateLocation> TemplateLocations { get; set; }
        public List<IDataLocation> SchemaMetaLocations { get; set; }
        public string TemplateFileExtension { get; set; }

        public Func<Dictionary<string, Func<object, string>>> GetTemplatesCache { get; set; }
        public Func<Dictionary<string, Func<object, string>>> SetTemplatesCache { get; set; }

        public Func<IRenderSettings, string> GetRenderedHtmlCache { get; set; }
        public Action<IRenderSettings, string> SetRenderedHtmlCache { get; set; }

        public List<string> ExcludeViewmodelsAtGeneration { get; set; }
        public List<string> AddNamespacesAtGeneration { get; set; }

    }

    public interface IDataLocation
    {
        string Name { get; set; }
        string Path { get; set; }
    }

    public class DataLocation : IDataLocation
    {
        public string Name { get; set; }
        public string Path { get; set; }
    }

}
