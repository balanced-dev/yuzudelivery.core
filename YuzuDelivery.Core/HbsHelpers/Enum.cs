using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HandlebarsDotNet;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace YuzuDelivery.Core
{
    public class Enum
    {
        public Enum()
        {
            HandlebarsDotNet.Handlebars.RegisterHelper("enum", (writer, context, parameters) =>
            {
                var jsonSettings = new JsonSerializerSettings
                {
                    Converters = new List<JsonConverter> { new StringEnumConverter() }
                };

                var enumValue = JsonConvert.SerializeObject(parameters[0], jsonSettings);
                if (enumValue != null)
                    enumValue = enumValue.Replace("\"", "");
                else
                    enumValue = string.Empty;

                if (enumValue.StartsWith("_"))
                    enumValue = enumValue.Substring(1);

                writer.WriteSafeString(enumValue);
            });
        }
    }
}
