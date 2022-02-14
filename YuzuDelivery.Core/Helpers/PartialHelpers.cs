using System;
using System.Collections.Generic;
using System.Linq;
using HandlebarsDotNet.Compiler;

namespace YuzuDelivery.Core.Helpers
{
    public static class PartialHelpers
    {
        public static bool IsSimple(this Type type)
        {
            return type.IsPrimitive
                   || type == typeof(string);
        }
        
        public static Dictionary<string, object> GetDataModel(object[] parameters, dynamic context = null)
        {
            var paramType = parameters[1].GetType();
            var properties = new Dictionary<string, object>();
            //we can't support modifiers and hashParameters on generic types
            if (paramType.IsSimple() || paramType.IsArray || paramType.IsGenericType)
            {
                return null;
            }
            
            var modifiers = parameters.Where((source, index) => index > 1)
                .Where(x => x != null && x.GetType().IsSimple())
                .Select(x => x.ToString()).ToList();
            
            if (modifiers.Any())
            {
                if (properties.Any(property => property.Key == "_modifiers" && property.Value == null))
                {
                    properties.Remove("_modifiers");
                }

                if (!properties.ContainsKey("_modifiers"))
                {
                    properties.Add("_modifiers", modifiers);
                }
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
            else if(context != null)
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

            return properties;
        }
    }
}