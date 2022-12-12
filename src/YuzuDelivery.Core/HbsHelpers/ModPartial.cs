using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using HandlebarsDotNet.Compiler;
using Newtonsoft.Json;
using YuzuDelivery.Core.Helpers;

namespace YuzuDelivery.Core
{
    public class ModPartial
    {
        /*
        var partial = Handlebars.partials[path];
                if (typeof partial !== 'function') {
                    partial = Handlebars.compile(partial);
                }
                return partial(context);
    */

        public ModPartial()
        {
            HandlebarsDotNet.Handlebars.RegisterHelper("modPartial", (writer, context, parameters) =>
            {
                if (parameters.Length >= 3)
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

                        _ref = vmType.GetBlockName();
                    }

                    var r = HandlebarsDotNet.Handlebars.Configuration.RegisteredTemplates
                        .Where(x => x.Key == _ref.RemoveFirstForwardSlash())
                        .Select(x => x.Value).FirstOrDefault();

                    if (r != null)
                    {
                        r(writer, PartialHelpers.GetDataModel(parameters) ?? parameters[1]);
                    }
                    else
                        throw new Exception(string.Format("Handlebars modifier partial cannot find partial {0}",
                            parameters[0]));
                }
                else
                    throw new Exception(
                        "Handlebars modifier partial should have 3 parameters; partial name, content and modifier");
            });
        }

        
    }


    [Obsolete("no longer in use?")]
    public static class ObjectExtensions
    {
        public static IDictionary<string, object> AddProperty(this object obj, string name, object value)
        {
            var dictionary = obj.ToDictionary();
            dictionary.Add(name, value);
            return dictionary;
        }

        // helper
        public static IDictionary<string, object> ToDictionary(this object obj)
        {
            IDictionary<string, object> result = new Dictionary<string, object>();
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(obj);
            foreach (PropertyDescriptor property in properties)
            {
                result.Add(property.Name, property.GetValue(obj));
            }

            return result;
        }
    }
}
