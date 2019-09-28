using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace YuzuDelivery.Core
{
    public static class ViewmodelHelperExtensions
    {

        public static string AsAlias(this PropertyInfo property)
        {
            return property.Name.AsAlias();
        }

        public static string AsAlias(this string propertyName)
        {
            return propertyName.FirstCharacterToLower();
        }

        public static string ToTemplate(this string viewmodelName)
        {
            return viewmodelName.RemoveAllVmPrefixes().FirstCharacterToLower();
        }

        public static string RemoveAllDefinitionPrefixes(this string str, bool removeForwardSlash = false)
        {
            var prefix = Yuzu.Configuration.BlockRefPrefix;
            if (removeForwardSlash)
            {
                prefix = Yuzu.Configuration.BlockRefPrefix.RemoveFirstForwardSlash();
                if (str.StartsWith(prefix))
                    return str.Substring(3);
            }
            else
            {
                if (str.StartsWith(prefix))
                    return str.Substring(4);
            }
            return str;
        }


        public static string RemoveAllVmPrefixes(this string str)
        {
            return str.Replace(Yuzu.Configuration.BlockPrefix, "").Replace(Yuzu.Configuration.SubPrefix, "").Replace(Yuzu.Configuration.PagePrefix, "");
        }

        public static string RemoveComponentVmPrefixes(this string str)
        {
            return str.Replace(Yuzu.Configuration.BlockPrefix, "").Replace(Yuzu.Configuration.PagePrefix, "");
        }

        public static bool IsPage(this string propertyTypeName)
        {
            return propertyTypeName.StartsWith(Yuzu.Configuration.PagePrefix);
        }

        public static bool IsComponentVm(this string propertyTypeName, bool allowIgnored = false)
        {
            if (allowIgnored)
                return (propertyTypeName.StartsWith(Yuzu.Configuration.BlockPrefix) || propertyTypeName.StartsWith(Yuzu.Configuration.PagePrefix));
            else
                return (propertyTypeName.StartsWith(Yuzu.Configuration.BlockPrefix) || propertyTypeName.StartsWith(Yuzu.Configuration.PagePrefix));
        }

        public static bool IsBlockOrSubVm(this string propertyTypeName)
        {
            return (propertyTypeName.StartsWith(Yuzu.Configuration.BlockPrefix) || propertyTypeName.StartsWith(Yuzu.Configuration.SubPrefix));
        }

        public static bool IsSubVm(this string propertyTypeName)
        {
            return (propertyTypeName.StartsWith(Yuzu.Configuration.SubPrefix));
        }

        public static string BlockRefToVmTypeName(this string refName)
        {
            return refName.Replace(Yuzu.Configuration.BlockRefPrefix, Yuzu.Configuration.BlockPrefix);
        }

        public static Type GetComponent(this Type type)
        {
            if (type.Name.IsComponentVm(true))
                return type;
            else
            {
                return Yuzu.Configuration.ViewModels.Where(x =>
                    x.GetProperties().Any(y => HasPropertyType(type, y)))
                    .FirstOrDefault();
            }
        }

        private static bool HasPropertyType(Type type, PropertyInfo p)
        {
            var propertyType = p.PropertyType;
            if (propertyType.IsGenericType)
            {
                var genType = propertyType.GenericTypeArguments.FirstOrDefault();
                if (genType == type)
                    return true;
                else if (genType != null && genType.Name.IsSubVm())
                    return genType.GetProperties().Any(x => HasPropertyType(type, x));
            }
            else
            {
                if (propertyType == type)
                    return true;
                else if (propertyType.Name.IsSubVm())
                    return propertyType.GetProperties().Any(x => HasPropertyType(type, x));
            }

            return false;
        }

    }
        
}
