using System;
using System.Collections.Generic;
#if NETCOREAPP
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
#else
using System.Web.Mvc;
#endif

namespace YuzuDelivery.Core
{
    public interface IYuzuDefinitionTemplates
    {
        string Render(IRenderSettings settings);
#if NETCOREAPP
        string Render<E>(object model, bool showJson = false, IRenderSettings settings = null, IHtmlHelper html = null, IDictionary<string, object> mappingItems = null);
#else
        string Render<E>(object model, bool showJson = false, IRenderSettings settings = null, HtmlHelper html = null, IDictionary<string, object> mappingItems = null);
#endif
        string GetSuspectTemplateName(Type model);
    }
}