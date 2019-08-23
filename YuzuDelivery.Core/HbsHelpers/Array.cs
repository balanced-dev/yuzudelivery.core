using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HandlebarsDotNet;

namespace YuzuDelivery.Umbraco.Blocks
{
    public class Array
    {
        public Array()
        {
            HandlebarsDotNet.Handlebars.RegisterHelper("array", (writer, options, context, parameters) =>
            {
                writer.WriteSafeString(parameters[0]);
            });
        }

    }
}
