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
        public virtual T From<T>(ResolutionContext context)
            where T : YuzuMappingContext
        {
            var output = new YuzuMappingContext();

            AddDefaults(output, context);

            return output as T;
        }

        protected void AddDefaults(YuzuMappingContext output, ResolutionContext context)
        {
            output.Items = context.Items;
            output.HttpContext = new HttpContextWrapper(HttpContext.Current);

            if (context.Items.ContainsKey("HtmlHelper"))
            {
                output.Html = context.Items["HtmlHelper"] as HtmlHelper;
            }
        }
    }
}
