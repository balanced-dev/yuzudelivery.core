using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using AutoMapper;
using System.Web;

namespace YuzuDelivery.Umbraco.Blocks
{
    public class YuzuDefinitionTemplates : IYuzuDefinitionTemplates
    {
        private const string TemplatesNotFoundMessage = "Yuzu definition template {0} not found";
        private const string RenderSettingsNotFound = "Render settings data not set";
        private const string RenderSettingsDataNotFound = "Render settings data not set for template {0}";

        private IMapper mapper;

        public YuzuDefinitionTemplates(IMapper mapper)
        {
            this.mapper = mapper;
        }

        public virtual string Render<E>(IRenderSettings settings, IDictionary<string, object> mappingItems = null)
        {
            if (settings.MapFrom != null)
            {
                if (mappingItems == null)
                    mappingItems = new Dictionary<string, object>();

                settings.Data = () => {
                    return mapper.Map<E>(settings.MapFrom, opt => {
                        foreach(var i in mappingItems) {
                            opt.Items.Add(i.Key, i.Value);
                        }
                    }); };
            }
            return Render(settings);
        }

        public virtual string Render(IRenderSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(RenderSettingsNotFound);

            string html = null;

            if (!string.IsNullOrEmpty(settings.CacheName) && Yuzu.Configuration.GetRenderedHtmlCache != null)
                html = Yuzu.Configuration.GetRenderedHtmlCache(settings);

            if (string.IsNullOrEmpty(html))
            {
               var data = CreateData(settings);

               html = RenderTemplate(settings, data);
               html = AddCurrentJsonToTemplate(settings, data, html);

               if (!string.IsNullOrEmpty(settings.CacheName) && Yuzu.Configuration.SetRenderedHtmlCache != null)
                    Yuzu.Configuration.SetRenderedHtmlCache(settings, html);
            }

            return html;
        }

        public virtual object CreateData(IRenderSettings settings)
        {
            if (settings.Data != null)
                return settings.Data();
            else
                return string.Empty;
        }

        public virtual string RenderTemplate(IRenderSettings settings, object data)
        {
            var templates = Yuzu.Configuration.GetTemplatesCache();
            if (templates == null)
            {
                templates = Yuzu.Configuration.SetTemplatesCache();
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
    }
}
