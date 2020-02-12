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
            var prefix = YuzuConstants.Configuration.BlockRefPrefix;
            if (removeForwardSlash)
            {
                prefix = YuzuConstants.Configuration.BlockRefPrefix.RemoveFirstForwardSlash();
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
            return str.Replace(YuzuConstants.Configuration.BlockPrefix, "").Replace(YuzuConstants.Configuration.SubPrefix, "").Replace(YuzuConstants.Configuration.PagePrefix, "");
        }

        public static string RemoveComponentVmPrefixes(this string str)
        {
            return str.Replace(YuzuConstants.Configuration.BlockPrefix, "").Replace(YuzuConstants.Configuration.PagePrefix, "");
        }

        public static bool IsPage(this string propertyTypeName)
        {
            return propertyTypeName.StartsWith(YuzuConstants.Configuration.PagePrefix);
        }

        public static bool IsComponentVm(this string propertyTypeName, bool allowIgnored = false)
        {
            if (allowIgnored)
                return (propertyTypeName.StartsWith(YuzuConstants.Configuration.BlockPrefix) || propertyTypeName.StartsWith(YuzuConstants.Configuration.PagePrefix));
            else
                return (propertyTypeName.StartsWith(YuzuConstants.Configuration.BlockPrefix) || propertyTypeName.StartsWith(YuzuConstants.Configuration.PagePrefix));
        }

        public static bool IsBlockOrSubVm(this string propertyTypeName)
        {
            return (propertyTypeName.StartsWith(YuzuConstants.Configuration.BlockPrefix) || propertyTypeName.StartsWith(YuzuConstants.Configuration.SubPrefix));
        }

        public static bool IsSubVm(this string propertyTypeName)
        {
            return (propertyTypeName.StartsWith(YuzuConstants.Configuration.SubPrefix));
        }

        public static string BlockRefToVmTypeName(this string refName)
        {
            return refName.Replace(YuzuConstants.Configuration.BlockRefPrefix, YuzuConstants.Configuration.BlockPrefix);
        }

        public static Type GetComponent(this Type type, IYuzuConfiguration config)
        {
            if (type.Name.IsComponentVm(true))
                return type;
            else
            {
                return config.ViewModels.Where(x =>
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
