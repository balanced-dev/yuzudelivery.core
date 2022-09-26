using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HandlebarsDotNet;

namespace YuzuDelivery.Core
{
    public class Enum
    {
        public Enum()
        {
            HandlebarsDotNet.Handlebars.RegisterHelper("enum", (writer, context, parameters) =>
            {
                writer.WriteSafeString(EnumResolver.Convert(parameters[0]));
            });
        }
    }
}
