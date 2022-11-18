using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Web;
using YuzuDelivery.Core.ViewModelBuilder;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using YuzuDelivery.Core.Mapping;

namespace YuzuDelivery.Core
{

    public class YuzuDefinitionTemplates : IYuzuDefinitionTemplates
    {
        private const string TemplatesNotFoundMessage = "Yuzu definition template {0} not found";
        private const string RenderSettingsNotFound = "Render settings data not set";
        private const string RenderSettingsDataNotFound = "Render settings data not set for template {0}";

        private readonly IMapper mapper;
        private readonly IYuzuConfiguration config;
        private readonly IEnumerable<IMapperAddItem> mapperAddItems;
        private readonly IYuzuTypeFactoryRunner typeFactoryRunner;

        public YuzuDefinitionTemplates(IMapper mapper, IYuzuConfiguration config, IEnumerable<IMapperAddItem> mapperAddItems, IYuzuTypeFactoryRunner typeFactoryRunner)
        {
            this.mapper = mapper;
            this.config = config;
            this.mapperAddItems = mapperAddItems;
            this.typeFactoryRunner = typeFactoryRunner;
        }
        public string Render<E>(object model, bool showJson = false, IRenderSettings settings = null, IHtmlHelper html = null, IDictionary<string, object> mappingItems = null)
        {
            if (settings == null)
                settings = new RenderSettings() { ShowJson = showJson };

            if (model != null)
            {
                if (mappingItems == null)
                    mappingItems = new Dictionary<string, object>();

                if(!mappingItems.ContainsKey("HtmlHelper") && html != null)
                    mappingItems.Add("HtmlHelper", html);

                foreach (var a in mapperAddItems)
                {
                    a.Add(mappingItems);
                }

                if (string.IsNullOrEmpty(settings.Template))
                    settings.Template = GetSuspectTemplateNameFromVm(typeof(E));

                settings.Data = () => {
                    var output = typeFactoryRunner.Run<E>(mappingItems);
                    if (output != null)
                        return output;
                    else
                        return mapper.Map<E>(model, mappingItems);
                };
            }
            return Render(settings);
        }

        public virtual string Render(IRenderSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(RenderSettingsNotFound);

            string html = null;

            if (!string.IsNullOrEmpty(settings.CacheName) && config.GetRenderedHtmlCache != null)
                html = config.GetRenderedHtmlCache(settings);

            if (string.IsNullOrEmpty(html))
            {
               var data = CreateData(settings);

               html = RenderTemplate(settings, data);
               html = AddCurrentJsonToTemplate(settings, data, html);

               if (!string.IsNullOrEmpty(settings.CacheName) && config.SetRenderedHtmlCache != null)
                    config.SetRenderedHtmlCache(settings, html);
            }

            return html;
        }

        public virtual object CreateData(IRenderSettings settings)
        {
            if (settings.Data != null)
                return settings.Data();
            else
                return new { };
        }

        public virtual string RenderTemplate(IRenderSettings settings, object data)
        {
            var templates = config.GetTemplatesCache();
            if (templates == null)
            {
                templates = config.SetTemplatesCache();
            }



            if (!templates.ContainsKey(settings.Template))
                throw new Exception(string.Format(TemplatesNotFoundMessage, settings.Template));

            return templates[settings.Template](data);
        }

        public virtual string AddCurrentJsonToTemplate(IRenderSettings settings, object data, string html)
        {
            if (settings.ShowJson && data != null && data.ToString() != string.Empty)
            {
                var json = JsonConvert.SerializeObject(data, Formatting.Indented);
                return string.Format("{0}<pre data-app=\"JSONHelper\">{1}</pre>", html, HttpUtility.HtmlEncode(json));
            }
            return html;
        }

        public string GetSuspectTemplateNameFromVm(Type vmType)
        {
            var suspectName = vmType.Name.RemoveAllVmPrefixes();

            //allow getting template name from base viewmodels
            if (vmType.BaseType != null && vmType.BaseType.Name.StartsWith("vm"))
                suspectName = vmType.BaseType.Name.RemoveAllVmPrefixes();

            if (vmType.Name.IsPage())
                return suspectName.FirstCharacterToLower();
            else
                return $"par{suspectName}";
        }
    }
}
