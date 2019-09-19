using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HandlebarsDotNet;
using Newtonsoft.Json;

namespace YuzuDelivery.Core
{
    public class ToString
    {
        public ToString()
        {
            HandlebarsDotNet.Handlebars.RegisterHelper("toString", (writer, context, parameters) =>
            {
                writer.WriteSafeString(JsonConvert.SerializeObject(parameters[0]));
            });
        }
    }
}
