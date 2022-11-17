using System;
using System.Collections.Generic;

namespace YuzuDelivery.Core
{
    internal class DefaultYuzuMapper : IMapper
    {
        private readonly AutoMapper.IMapper _inner;

        public DefaultYuzuMapper(AutoMapper.MapperConfigurationExpression cfg)
        {
            var config = new AutoMapper.MapperConfiguration(cfg);
            _inner = new AutoMapper.Mapper(config);
        }

        public object Concrete => _inner;

        public TDest Map<TDest>(object source)
        {
            return _inner.Map<TDest>(source);
        }

        public TDest Map<TDest>(object source, IDictionary<string, object> items)
        {
            return _inner.Map<TDest>(source, opt => {
                foreach (var i in items)
                {
                    opt.Items[i.Key] = i.Value;
                }
            });
        }

        public object Map(object source, Type sourceType, Type destinationType, IDictionary<string, object> items)
        {
            return _inner.Map(source, sourceType, destinationType, opt => {
                foreach (var i in items)
                {
                    opt.Items.Add(i.Key, i.Value);
                }
            });
        }
    }
}
