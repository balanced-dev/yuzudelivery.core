using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Reflection;
using YuzuDelivery.Core.Mapping;

namespace YuzuDelivery.Core
{
    public class YuzuConfiguration : IYuzuConfiguration
    {
        public YuzuConfiguration(IEnumerable<IUpdateableConfig> extraConfigs)
        {
            SchemaMetaLocations = new List<IDataLocation>();
            TemplateLocations = new List<ITemplateLocation>();

            MappingAssemblies = new List<Assembly>();

            InstalledManualMaps = new List<ManualMapInstalledType>();
            ActiveManualMaps = new List<ManualMapActiveType>();
            ViewmodelFactories = new Dictionary<Type, Func<IYuzuTypeFactory>>();

            foreach(var i in extraConfigs)
            {
                MappingAssemblies = MappingAssemblies.Union(i.MappingAssemblies).ToList();
            }
        }

        public List<Assembly> MappingAssemblies { get; set; }
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
                ViewModels = value.SelectMany(x => x.GetTypes().Where(y => y.Name.IsVm()));
            }
        }

        public virtual IEnumerable<Type> ViewModels { get; private set; }
        public IEnumerable<Type> CMSModels { get; set; }

        public List<ManualMapInstalledType> InstalledManualMaps { get; private set; }
        public List<ManualMapActiveType> ActiveManualMaps { get; private set; }
        public Dictionary<Type, Func<IYuzuTypeFactory>> ViewmodelFactories { get; private set; }

        public List<ITemplateLocation> TemplateLocations { get; set; }
        public List<IDataLocation> SchemaMetaLocations { get; set; }

        public Func<Dictionary<string, Func<object, string>>> GetTemplatesCache { get; set; }
        public Func<Dictionary<string, Func<object, string>>> SetTemplatesCache { get; set; }

        public Func<IRenderSettings, string> GetRenderedHtmlCache { get; set; }
        public Action<IRenderSettings, string> SetRenderedHtmlCache { get; set; }

        public void AddActiveManualMap<Resolver, Dest>(string destMemberName = null)
        {
            ActiveManualMaps.Add(new ManualMapActiveType()
            {
                Resolver = typeof(Resolver),
                Interface = typeof(Resolver).GetInterfaces().Where(x => !x.GetGenericArguments().Any()).FirstOrDefault(),
                Dest = typeof(Dest),
                DestMemberName = destMemberName
            });
        }

        public bool HasActiveManualMap(string dest, string destMemberName = null)
        {
            return ActiveManualMaps.Any(x => x.Dest.Name == dest && x.DestMemberName == destMemberName);
        }

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

    public abstract class UpdateableConfig : IUpdateableConfig
    {
        public UpdateableConfig()
        {
            MappingAssemblies = new List<Assembly>();
        }

        public List<Assembly> MappingAssemblies { get; set; }
    }

    public class ManualMapInstalledType
    {
        public Type Interface { get; set; }
        public Type Concrete { get; set; }
        public Type SourceType { get; set; }
        public Type SourceMemberType { get; set; }
        public Type DestType { get; set; }
        public Type DestMemberType { get; set; }
    }

    public class ManualMapActiveType
    {
        public Type Resolver { get; set; }
        public Type Interface { get; set; }
        public Type Dest { get; set; }
        public string DestMemberName { get; set; }
    }

}
