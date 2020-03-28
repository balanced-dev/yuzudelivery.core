using System.Collections.Generic;
using System.Web.Mvc;

namespace YuzuDelivery.Core
{
    public interface IYuzuDefinitionTemplates
    {
        string Render(IRenderSettings settings);
        string Render<E>(IRenderSettings settings, HtmlHelper html = null, IDictionary<string, object> mappingItems = null);
    }
}