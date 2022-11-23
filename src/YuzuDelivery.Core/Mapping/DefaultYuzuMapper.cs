using System;
using System.Collections.Generic;

namespace YuzuDelivery.Core.Mapping
{
    internal class DefaultYuzuMapper : IMapper
    {
        private readonly global::AutoMapper.IMapper _inner;

        public DefaultYuzuMapper(global::AutoMapper.IMapper inner)
        {
            _inner = inner;
        }

        public object Concrete => _inner;

        public TDest Map<TDest>(object source)
        {
            return _inner.Map<TDest>(source, _ => { });
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
                    opt.Items[i.Key] = i.Value;
                }
            });
        }
    }
}
