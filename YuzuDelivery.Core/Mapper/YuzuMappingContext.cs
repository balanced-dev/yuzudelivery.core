using System.Web.Mvc;
using System.Collections.Generic;
using System.Web;

namespace YuzuDelivery.Core
{
    public class YuzuMappingContext
    {
        public IDictionary<string, object> Items;
        public HtmlHelper Html { get; set; }
        public HttpContextBase HttpContext { get; set; }
    }
}
