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
            SchemaMetaLocations = new List<IDataLocation>();
            TemplateLocations = new List<ITemplateLocation>();
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

        public IEnumerable<Type> CMSModels { get; set; }

        public List<ITemplateLocation> TemplateLocations { get; set; }
        public List<IDataLocation> SchemaMetaLocations { get; set; }

        public Func<Dictionary<string, Func<object, string>>> GetTemplatesCache { get; set; }
        public Func<Dictionary<string, Func<object, string>>> SetTemplatesCache { get; set; }

        public Func<IRenderSettings, string> GetRenderedHtmlCache { get; set; }
        public Action<IRenderSettings, string> SetRenderedHtmlCache { get; set; }

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
