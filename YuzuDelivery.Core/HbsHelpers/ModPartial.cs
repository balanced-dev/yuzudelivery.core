using System;
using System.Dynamic;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;

namespace YuzuDelivery.Umbraco.Blocks
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
                if (parameters.Length == 3)
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

                    if (r != null)
                    {
                        if (parameters[1] is ExpandoObject)
                        {
                            dynamic outputContext = parameters[1];
                            outputContext.modifier = parameters[2];
                            r(writer, outputContext);
                        }
                        else if (parameters[1].GetType().GetProperties().Any(x => x.Name.ToLower() == "modifier"))
                        {
                            var modifier = parameters[1].GetType().GetProperties().Where(x => x.Name.ToLower() == "modifier").FirstOrDefault();
                            modifier.SetValue(parameters[1], parameters[2]);
                            r(writer, parameters[1]);
                        }
                        else
                        {
                            var contentWithModifier = parameters[1].AddProperty("modifier", parameters[2]);
                            r(writer, parameters[1]);
                        }
                    }
                    else
                        throw new Exception(string.Format("Handlebars modifier partial cannot find partial {0}", parameters[0]));
                }
                else
                    throw new Exception("Handlebars modifier partial should have 3 parameters; parial name, content and modifier");
            });
        }


    }

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
