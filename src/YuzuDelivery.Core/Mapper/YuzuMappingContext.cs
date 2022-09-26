using System.Collections.Generic;
using System.Web;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;



namespace YuzuDelivery.Core
{
    public class YuzuMappingContext
    {
        public IDictionary<string, object> Items;
        public IHtmlHelper Html { get; set; }
        public HttpContext HttpContext { get; set; }
    }
}
