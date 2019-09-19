using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Reflection;

namespace YuzuDelivery.Core
{
    public static class ConfigTypesAndPropertiesExtensions
    {
        public static void Add<T>(this List<string> config)
        {
            if (config != null)
            {
                config.Add(typeof(T).Name);
            }
        }

        public static void Add<TSource, TProperty>(this Dictionary<string, string> config, Expression<Func<TSource, TProperty>> propertyLambda)
        {
            Type type = typeof(TSource);

            MemberExpression member = propertyLambda.Body as MemberExpression;
            if (member == null)
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a method, not a property.",
                    propertyLambda.ToString()));

            PropertyInfo propInfo = member.Member as PropertyInfo;
            if (propInfo == null)
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a field, not a property.",
                    propertyLambda.ToString()));

            config.Add(type.Name, propInfo.Name);
        }
    }
}
