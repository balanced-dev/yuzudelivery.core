using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YuzuDelivery.Core
{
    public interface IMapperAddItem
    {
        void Add(IDictionary<string, object> items);
    }
}
