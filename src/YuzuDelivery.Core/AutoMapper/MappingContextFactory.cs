using System;
using System.Collections.Generic;
using System.Reflection;
using AutoMapper;
using System.Web;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;

namespace YuzuDelivery.Core
{
    public class MappingContextFactory : IMappingContextFactory
    {

        private readonly IHttpContextAccessor _httpContextAccessor;

        public MappingContextFactory(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }


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
            output.HttpContext = _httpContextAccessor.HttpContext;

            if (items.ContainsKey("HtmlHelper"))
            {
                output.Html = items["HtmlHelper"] as IHtmlHelper;
            }
        }
    }
}
