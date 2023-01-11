using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace YuzuDelivery.Core
{
    public static class ConfigTypesAndPropertiesExtensions
    {
        public static void Add<T>(this IList<string> config)
        {
            if (config != null)
            {
                config.Add(typeof(T).Name);
            }
        }

        public static void Add<TSource, TProperty>(this IList<KeyValuePair<string,string>> config, Expression<Func<TSource, TProperty>> propertyLambda)
        {
            if (propertyLambda.Body is not MemberExpression member)
            {
                throw new ArgumentException($"Expression '{propertyLambda}' refers to a method, not a property.");
            }

            if (member.Member is not PropertyInfo propInfo)
            {
                throw new ArgumentException($"Expression '{propertyLambda}' refers to a field, not a property.");
            }


            config.Add(typeof(TSource).Name, propInfo.Name);
        }

        public static void Add(this IList<KeyValuePair<string, string>> config, string key, string value)
        {
            var element = new KeyValuePair<string, string>(key, value);
            config.Add(element);
        }
    }
}
