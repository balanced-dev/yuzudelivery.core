using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HandlebarsDotNet;

namespace YuzuDelivery.Core
{
    public class PictureSource
    {
        public string[] configSettings;

        public PictureSource()
        {

            // Define properties which are not used by image processor to be filtered out of query string
            configSettings = new string[]
            {
                "createWebP",
                "webPQuality",
                "createHighDensityDisplay",
                "highDensityDisplayDensity",
                "highDensityDimensionMultiplier",
                "highDensityQuality",
                "highDensityWebPQuality"
            };

            HandlebarsDotNet.Handlebars.RegisterHelper("pictureSource", (writer, context, parameters) =>
            {
                var sourceTagsHTML = string.Empty;

                if (parameters.Length >= 3)
                {
                    var mediaCondition = parameters[0].ToString();
                    var imageSrc = parameters[1] != null ? parameters[1].ToString() : string.Empty;
                    var settings = GetDefaultSettings();
                    var userSettings = new Dictionary<string, object>(parameters[2] as Dictionary<string, object>);

                    // Allows overriding of any settings and/or addition of extra settings to be used by the image processor
                    settings = Sanitize(Extend(settings, userSettings));

                    // Create webP first        
                    if (settings["createWebP"].ToBool())
                    {
                        // Create copy of object to prevent 
                        var webPSettings = ObjectClone(settings);

                        // Override settings to force webp format and use webP quality
                        webPSettings["format"] = "webp";
                        webPSettings["quality"] = settings["webPQuality"];

                        sourceTagsHTML += CreateSourceTag(mediaCondition, imageSrc, webPSettings, " type=\"image/webp\">");
                    }

                    // Create fallback image where webP is not supported
                    sourceTagsHTML += CreateSourceTag(mediaCondition, imageSrc, settings, ">");

                }

                writer.WriteSafeString(sourceTagsHTML);
            });
        }
        /// <summary>
        /// Sanitizes settings dictionary-values by removing whitespace characters
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>
        /// Sanitized settings dictionary
        /// </returns>
        private static Dictionary<string, object> Sanitize(Dictionary<string, object> obj)
        {
            foreach (string key in obj.Keys.ToList())
                obj[key] = obj[key].ToString()?.Trim();
            return obj;
        }

        public Dictionary<string, object> Extend(Dictionary<string, object> obj, Dictionary<string, object> src)
        {
            foreach (var i in src)
            {
                if (src[i.Key] != null)
                {
                    obj[i.Key] = src[i.Key];
                }
            };
            return obj;
        }

        public Dictionary<string, object> ObjectClone(Dictionary<string, object> src)
        {
            return new Dictionary<string, object>(src);
        }

        // Filter all default settings from parameters
        public Dictionary<string, object> GetImageProcessorSettings(Dictionary<string, object> obj)
        {
            foreach (var i in configSettings)
            {
                obj.Remove(i);
            }
            return obj;
        }

        public string CreateImageQueryString(Dictionary<string, object> settings)
        {
            var queryString = "?";
            var filteredSettings = GetImageProcessorSettings(ObjectClone(settings));

            foreach (var i in filteredSettings)
            {
                if (filteredSettings[i.Key] != null)
                {
                    queryString += queryString.Length > 1 ? "&" : "";
                    queryString += i.Key + "=" + filteredSettings[i.Key];
                }
            }

            return queryString;
        }

        public string CreateSourceTag(string mediaCondition, string imageSrc, Dictionary<string, object> settings, string tagClose)
        {
            var tagOpen = "<source media=\"(" + mediaCondition + ")\" srcset=\"";
            var imageSrcSeparator = ", ";
            var outputHTML = "";
            var highDensitySettings = ObjectClone(settings);

            // Open source tag and add standard density image source
            outputHTML += tagOpen + imageSrc + CreateImageQueryString(settings);

            // Check if a higher display density image source needs creating
            if (settings["createHighDensityDisplay"].ToBool())
            {

                // Apply multiplier to any dimension settings present
                if (highDensitySettings.ContainsKey("height"))
                {
                    highDensitySettings["height"] = highDensitySettings["height"].ToInt() * settings["highDensityDimensionMultiplier"].ToInt();
                }
                if (highDensitySettings.ContainsKey("width"))
                {
                    highDensitySettings["width"] = highDensitySettings["width"].ToInt() * settings["highDensityDimensionMultiplier"].ToInt();
                }
                outputHTML += imageSrcSeparator + imageSrc + CreateImageQueryString(highDensitySettings) + ' ' + settings["highDensityDisplayDensity"];
            }

            // Close off source tag accordingly (end srcset attribute, add type attribute for webp)
            outputHTML += '"';
            outputHTML += tagClose;

            return outputHTML;
        }

        public Dictionary<string, object> GetDefaultSettings()
        {
            // Define default settings
            var settings = new Dictionary<string, object>();
            settings.Add("quality", 80);
            settings.Add("createWebP", true);
            settings.Add("webPQuality", 85);
            settings.Add("createHighDensityDisplay", true);
            settings.Add("highDensityDisplayDensity", "1.5x");
            settings.Add("highDensityDimensionMultiplier", 2);
            settings.Add("highDensityQuality", 50);
            settings.Add("highDensityWebPQuality", 60);
            return settings;
        }

    }
}
