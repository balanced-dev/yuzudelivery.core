using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YuzuDelivery.Core
{
    public interface IMapper
    {
        object Concrete { get; }
        E Map<E>(object source);
        E Map<E>(object source, IDictionary<string, object> items);
        object Map(object source, Type sourceType, Type destinationType, IDictionary<string, object> items);
    }
}
