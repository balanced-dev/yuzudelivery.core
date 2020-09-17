using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HandlebarsDotNet;

namespace YuzuDelivery.Core
{
    public class ToLowerCase
    {
        public ToLowerCase()
        {
            HandlebarsDotNet.Handlebars.RegisterHelper("toLowerCase", (writer, context, parameters) =>
            {
                writer.WriteSafeString(parameters[0].ToString().ToLower());
            });
        }
    }
}
