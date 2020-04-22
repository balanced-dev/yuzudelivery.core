using AutoMapper;
using AutoMapper.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace YuzuDelivery.Core
{
    public class AddedMapContext
    {
        public AddedMapContext()
        {
            Items = new List<AddedMapContextItem>();
        }

        public List<AddedMapContextItem> Items { get; set; }

        public bool Has(Type source, Type dest)
        {
            return Items.Any(x => x.Source == source && x.Dest == dest);
        }

        public IMappingExpression<Source, Dest> AddOrGet<Source, Dest>(MapperConfigurationExpression cfg)
        {
            var map =  Items
                .Where(x => x.Source == typeof(Source) && x.Dest == typeof(Dest))
                .Select(x => x.Map as IMappingExpression<Source, Dest>)
                .FirstOrDefault();

            if (map == null)
            {
                map = cfg.CreateMap<Source, Dest>();
                Add(map);
            }

            return map;
        }

        public void Add<Source,Dest>(IMappingExpression<Source, Dest> map)
        {
            Items.Add(new AddedMapContextItem() { Source = typeof(Source), Dest = typeof(Dest), Map = map });
        }
    }

    public class AddedMapContextItem
    {
        public Type Source { get; set; }
        public Type Dest { get; set; }
        public object Map { get; set; }
    }

}
