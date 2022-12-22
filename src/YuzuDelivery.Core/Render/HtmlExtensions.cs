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
        public static IHtmlContent RenderYuzu<E>(this IHtmlHelper html, object model, string templateName = null, IDictionary<string, object> mappingItems = null, bool showJson = false)
        {
            mappingItems ??= new Dictionary<string, object>();

            if (!mappingItems.ContainsKey("HtmlHelper"))
            {
                mappingItems.Add("HtmlHelper", html);
            }

            var mapper = html.ViewContext.HttpContext.RequestServices.GetRequiredService<IMapper>();
            var factory = html.ViewContext.HttpContext.RequestServices.GetRequiredService<IYuzuTypeFactoryRunner>();
            var mapped = factory.Run<E>(mappingItems) ?? mapper.Map<E>(model, mappingItems);

            return html.RenderYuzu(templateName ?? typeof(E).GetTemplateName(), mapped, showJson);
        }

        // ReSharper disable once MemberCanBePrivate.Global - used downstream
        public static IHtmlContent RenderYuzu(this IHtmlHelper html, string templateName, object model, bool showJson = false)
        {
            var templateEngine = html.ViewContext.HttpContext.RequestServices.GetRequiredService<IYuzuTemplateEngine>();
            var markup = templateEngine.Render(templateName, model);

            // ReSharper disable once InvertIf
            if (showJson)
            {
                var json = JsonConvert.SerializeObject(model, Formatting.Indented);
                markup = $"{markup}<pre data-app=\"JSONHelper\">{HttpUtility.HtmlEncode(json)}</pre>";
            }

            return html.Raw(markup);
        }
    }
}
