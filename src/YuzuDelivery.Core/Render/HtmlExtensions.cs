using System;
using System.Collections.Generic;
using System.Web;
using YuzuDelivery.Core;
using Microsoft.AspNetCore.Html;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using YuzuDelivery.Core.Mapping;

// ReSharper disable once CheckNamespace
namespace Microsoft.AspNetCore.Mvc.Rendering
{
    public static class HtmlExtensions
    {
        // ReSharper disable once MemberCanBePrivate.Global - used downstream
        [Obsolete("This should probably be called MapAndRenderYuzu")]
        public static IHtmlContent RenderYuzu<E>(this IHtmlHelper html, object model, string templateName = null, IDictionary<string, object> mappingItems = null)
        {
            mappingItems ??= new Dictionary<string, object>();

            if (!mappingItems.ContainsKey("HtmlHelper"))
            {
                mappingItems.Add("HtmlHelper", html);
            }

            var mapper = html.ViewContext.HttpContext.RequestServices.GetRequiredService<IMapper>();
            var factory = html.ViewContext.HttpContext.RequestServices.GetRequiredService<IYuzuTypeFactoryRunner>();
            var mapped = factory.Run<E>(mappingItems) ?? mapper.Map<E>(model, mappingItems);

            return html.RenderYuzu(templateName ?? typeof(E).GetTemplateName(), mapped);
        }

        // ReSharper disable once MemberCanBePrivate.Global - used downstream
        public static IHtmlContent RenderYuzu(this IHtmlHelper html, string templateName, object model)
        {
            var templateEngine = html.ViewContext.HttpContext.RequestServices.GetRequiredService<IYuzuTemplateEngine>();
            var markup = templateEngine.Render(templateName, model);

            return html.Raw(markup);
        }

        // ReSharper disable once MemberCanBePrivate.Global - used downstream
        public static IHtmlContent RenderYuzu(this IHtmlHelper html, IYuzuViewModel model, string templateName = null)
        {
            templateName ??= model.GetType().GetTemplateName();

            var templateEngine = html.ViewContext.HttpContext.RequestServices.GetRequiredService<IYuzuTemplateEngine>();
            var markup = templateEngine.Render(templateName, model);

            return html.Raw(markup);
        }
    }
}
