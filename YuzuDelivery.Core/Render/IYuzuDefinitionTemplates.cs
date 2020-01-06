using System.Collections.Generic;

namespace YuzuDelivery.Core
{
    public interface IYuzuDefinitionTemplates
    {
        string Render(IRenderSettings settings);
        string Render<E>(IRenderSettings settings, IDictionary<string, object> mappingItems);
    }
}