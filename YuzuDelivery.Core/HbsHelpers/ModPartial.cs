using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using HandlebarsDotNet.Compiler;

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

                        _ref = vmType.Name.Replace("vmBlock_", "par");
                    }

                    var r = HandlebarsDotNet.Handlebars.Configuration.RegisteredTemplates
                        .Where(x => x.Key == _ref)
                        .Select(x => x.Value).FirstOrDefault();

                    if (r != null)
                    {
                        if (_ref == "parMasonryLoadMore")
                        {
                            var test = "";
                        }
                        r(writer, GetDataModel(parameters));
                    }
                    else
                        throw new Exception(string.Format("Handlebars modifier partial cannot find partial {0}",
                            parameters[0]));
                }
                else
                    throw new Exception(
                        "Handlebars modifier partial should have 3 parameters; parial name, content and modifier");
            });
        }

        private static Dictionary<string, object> GetDataModel(object[] parameters)
        {
            var modifiers = parameters.Where((source, index) => index > 1)
                .Where(x => x != null && x.GetType() != typeof(HashParameterDictionary))
                .Select(x => x.ToString()).ToList();


            //not sure what is faster
            // linq:
            var properties = parameters[1].GetType().GetProperties().ToDictionary(
                property => StringExtensions.FirstCharacterToLower(property.Name),
                property => property.GetValue(parameters[1]));

            //jsonSerialize/deserialize:
            //var json = JsonConvert.SerializeObject(parameters[1]);
            //var properties = JsonConvert.DeserializeObject<Dictionary<string, object>>(json); 

            if (properties.Any(property => property.Key == "_modifiers" && property.Value == null))
            {
                properties.Remove("_modifiers");
            }

            if (!properties.ContainsKey("_modifiers"))
            {
                properties.Add("_modifiers", modifiers);
            }
            
            if (!(parameters[parameters.Length - 1] is HashParameterDictionary hashParameterDictionary))
                return properties;
            
            foreach ( var parameter in hashParameterDictionary)
            {
                properties.Add(parameter.Key, parameter.Value);
            }
            return properties;
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
