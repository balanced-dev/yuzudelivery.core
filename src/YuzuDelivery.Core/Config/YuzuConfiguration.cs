using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using YuzuDelivery.Core.Mapping;

namespace YuzuDelivery.Core
{
    public class YuzuConfiguration
    {
        public IList<Assembly> MappingAssemblies { get; } = new List<Assembly>();
        public IList<Assembly> ViewModelAssemblies { get; } = new List<Assembly>();
        public IList<Type> ViewModels { get; } = new List<Type>();
        public IList<Type> CMSModels { get; } = new List<Type>();

        public IList<ManualMapInstalledType> InstalledManualMaps { get; } = new List<ManualMapInstalledType>();
        public IList<ManualMapActiveType> ActiveManualMaps { get;  }= new List<ManualMapActiveType>();

        public IDictionary<Type, Func<IYuzuTypeFactory>> ViewmodelFactories { get; } = new Dictionary<Type, Func<IYuzuTypeFactory>>();

        public IList<string> BaseSiteConfigFiles { get; } = new List<string>();


        public void AddActiveManualMap<Resolver, Dest>(string destMemberName = null)
        {
            ActiveManualMaps.Add(new ManualMapActiveType()
            {
                Resolver = typeof(Resolver),
                Interface = typeof(Resolver).GetInterfaces().FirstOrDefault(x => !x.GetGenericArguments().Any()),
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
