using System.Collections.Generic;
using System.Web;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;

namespace YuzuDelivery.Core
{
    public class YuzuMappingContext
    {
        public IDictionary<string, object> Items { get; }

        public YuzuMappingContext(IDictionary<string, object> items)
        {
            Items = items;
        }
    }
}
