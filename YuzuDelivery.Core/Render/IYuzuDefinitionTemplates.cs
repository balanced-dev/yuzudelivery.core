using System.Collections.Generic;

namespace YuzuDelivery.Core
{
    public interface IYuzuDefinitionTemplates
    {
        string AddCurrentJsonToTemplate(IRenderSettings settings, object data, string html);
        object CreateData(IRenderSettings settings);
        string Render(IRenderSettings settings);
        string Render<E>(IRenderSettings settings, IDictionary<string, object> mappingItems);
        string RenderTemplate(IRenderSettings settings, object data);
    }
}