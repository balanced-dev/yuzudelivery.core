using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace YuzuDelivery.Core
{
    public interface IYuzuDefinitionTemplates
    {
        string Render(IRenderSettings settings);
        string Render<E>(object model, bool showJson = false, IRenderSettings settings = null, HtmlHelper html = null, IDictionary<string, object> mappingItems = null);
        string GetSuspectTemplateName(Type model);
    }
}