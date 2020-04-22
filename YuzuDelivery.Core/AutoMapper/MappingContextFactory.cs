using System;
using System.Collections.Generic;
using System.Reflection;
using AutoMapper;
using System.Web.Mvc;
using System.Web;

namespace YuzuDelivery.Core
{
    public class MappingContextFactory : IMappingContextFactory
    {

        public virtual T From<T>(IDictionary<string, object> items)
            where T : YuzuMappingContext
        {
            var output = new YuzuMappingContext();

            AddDefaults(output, items);

            return output as T;
        }

        protected void AddDefaults(YuzuMappingContext output, IDictionary<string, object> items)
        {
            output.Items = items;
            output.HttpContext = new HttpContextWrapper(HttpContext.Current);

            if (items.ContainsKey("HtmlHelper"))
            {
                output.Html = items["HtmlHelper"] as HtmlHelper;
            }
        }
    }
}
