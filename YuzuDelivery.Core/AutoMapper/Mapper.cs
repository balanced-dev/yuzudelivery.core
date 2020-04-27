using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using A = AutoMapper;

namespace YuzuDelivery.Core
{
    public class AutoMapperIntegration : IMapper
    {
        private readonly A.IMapper mapper;

        public AutoMapperIntegration(A.IMapper mapper)
        {
            this.mapper = mapper;
        }

        public object Concrete { get { return mapper; } }

        public E Map<E>(object source)
        {
            return mapper.Map<E>(source);
        }

        public E Map<E>(object source, IDictionary<string, object> items)
        {
            return mapper.Map<E>(source, opt => {
                foreach (var i in items) {
                    opt.Items.Add(i.Key, i.Value);
                }
            });
        }

        public object Map(object source, Type sourceType, Type destinationType, IDictionary<string, object> items)
        {
            return mapper.Map(source, sourceType, destinationType, opt => {
                foreach (var i in items)
                {
                    opt.Items.Add(i.Key, i.Value);
                }
            });
        }
    }
}
