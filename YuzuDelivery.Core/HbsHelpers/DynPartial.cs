using System;
using System.Collections.Generic;
using System.Linq;
using HandlebarsDotNet;
using HandlebarsDotNet.Compiler;
using Newtonsoft.Json;
using YuzuDelivery.Core.Helpers;

namespace YuzuDelivery.Core
{
    public class DynPartial
    {
        public DynPartial()
        {
            HandlebarsDotNet.Handlebars.RegisterHelper("dynPartial", (writer, context, parameters) =>
            {
                var _ref = string.Empty;
                if (parameters[0] != null)
                    _ref = parameters[0].ToString();

                if (_ref == string.Empty)
                {
                    var vmType = parameters[1].GetType();
                    if (vmType.IsArray)
                    {
                        vmType = vmType.GetElementType();
                    }

                    if (vmType.IsGenericType)
                    {
                        vmType = vmType.GetGenericArguments().FirstOrDefault();
                    }

                    _ref = vmType.Name.Replace("vmBlock_", "par");
                }

                var r = HandlebarsDotNet.Handlebars.Configuration.RegisteredTemplates
                    .Where(x => x.Key == _ref)
                    .Select(x => x.Value).FirstOrDefault();

                if (r == null)
                    throw new Exception(string.Format("dynPartial error : Partial not found for ref {0}", _ref));

                if (parameters.Length > 1)
                {
                    r(writer, PartialHelpers.GetDataModel(parameters, context) ?? parameters[1]);
                }
                else
                {
                    r(writer, context);
                }
            });
        }
    }
}