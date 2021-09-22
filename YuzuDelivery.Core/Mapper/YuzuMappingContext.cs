using System.Collections.Generic;
using System.Web;
#if NETCOREAPP
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
#else
using System.Web.Mvc;
#endif



namespace YuzuDelivery.Core
{
    public class YuzuMappingContext
    {
        public IDictionary<string, object> Items;

#if NETCOREAPP
        public IHtmlHelper Html { get; set; }
        public HttpContext HttpContext { get; set; }
#else
        public HtmlHelper Html { get; set; }
        public HttpContextBase HttpContext { get; set; }
#endif
    }
}
