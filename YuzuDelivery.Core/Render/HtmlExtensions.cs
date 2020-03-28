using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuzuDelivery.Core;

namespace System.Web.Mvc
{
    public static class HtmlExtensions
    {

        public static IHtmlString RenderYuzu<E>(this HtmlHelper helper, object model, IRenderSettings settings)
        {
            var fe = DependencyResolver.Current.GetService<IYuzuDefinitionTemplates>();
            return helper.Raw(fe.Render<E>(model, false, settings, helper, null));
        }

        public static IHtmlString RenderYuzu<E>(this HtmlHelper helper, object model, IDictionary<string, object> mappingItems, IRenderSettings settings = null)
        {
            var fe = DependencyResolver.Current.GetService<IYuzuDefinitionTemplates>();
            return helper.Raw(fe.Render<E>(model, false, settings, helper, mappingItems));
        }

        public static IHtmlString RenderYuzu<E>(this HtmlHelper helper, object model, bool showJson = false, IDictionary<string, object> mappingItems = null)
        {
            var fe = DependencyResolver.Current.GetService<IYuzuDefinitionTemplates>();
            return helper.Raw(fe.Render<E>(model, showJson, null, helper, mappingItems));
        }

        public static IHtmlString RenderYuzu(this HtmlHelper helper, IRenderSettings settings)
        {
            var fe = DependencyResolver.Current.GetService<IYuzuDefinitionTemplates>();
            return helper.Raw(fe.Render(settings));
        }
    }
}
