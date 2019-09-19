using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuzuDelivery.Core;

namespace System.Web.Mvc
{
    public static class HtmlExtensions
    {
        public static IHtmlString RenderFETemplate<E>(this HtmlHelper helper, IRenderSettings settings, IDictionary<string, object> mappingItems = null)
        {
            var fe = DependencyResolver.Current.GetService<IYuzuDefinitionTemplates>();
            return helper.Raw(fe.Render<E>(settings, mappingItems));
        }

        public static IHtmlString RenderFETemplate(this HtmlHelper helper, IRenderSettings settings)
        {
            var fe = DependencyResolver.Current.GetService<IYuzuDefinitionTemplates>();
            return helper.Raw(fe.Render(settings));
        }
    }
}
