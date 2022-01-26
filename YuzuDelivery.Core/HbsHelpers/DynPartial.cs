using System;
using System.Collections.Generic;
using System.Linq;
using HandlebarsDotNet;
using HandlebarsDotNet.Compiler;
using Newtonsoft.Json;

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
                    r(writer, GetDataModel(parameters, context) ?? parameters[1]);
                }
                else
                {
                    r(writer, context);
                }
            });
        }

        private static bool IsSimple(Type type)
        {
            return type.IsPrimitive
                   || type == typeof(string);
        }

        private static Dictionary<string, object> GetDataModel(object[] parameters, dynamic context)
        {
            var properties = new Dictionary<string, object>();

            var paramType = parameters[1].GetType();
            // we cannot support hashParameters when the base type is a simple type
            if (IsSimple(paramType))
            {
                return null;
            }
            
            if (paramType != typeof(HashParameterDictionary))
            {
                //not sure what is faster
                // linq:
                properties = paramType.GetProperties().ToDictionary(
                    property => StringExtensions.FirstCharacterToLower(property.Name),
                    property => property.GetValue(parameters[1]));

                //jsonSerialize/deserialize:
                //var json = JsonConvert.SerializeObject(parameters[1]);
                //var properties = JsonConvert.DeserializeObject<Dictionary<string, object>>(json); 
            }
            // when context is implicitly given
            else
            {
                properties = ((object)context).GetType().GetProperties().ToDictionary(
                    property => StringExtensions.FirstCharacterToLower(property.Name),
                    property => property.GetValue(context));
            }

            if (!(parameters[parameters.Length - 1] is HashParameterDictionary hashParameterDictionary))
                return properties;

            foreach (var parameter in hashParameterDictionary)
            {
                properties.Add(parameter.Key, parameter.Value);
            }

            return  properties;
        }
    }
}