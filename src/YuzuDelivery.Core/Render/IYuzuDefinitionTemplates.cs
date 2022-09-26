using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;

namespace YuzuDelivery.Core
{
    public interface IYuzuDefinitionTemplates
    {
        string Render(IRenderSettings settings);

        string Render<E>(object model, bool showJson = false, IRenderSettings settings = null, IHtmlHelper html = null, IDictionary<string, object> mappingItems = null);

        string GetSuspectTemplateNameFromVm(Type vmType);
    }
}