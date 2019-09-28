using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace YuzuDelivery.Core
{
    public class SchemaMetaPropertyService : ISchemaMetaPropertyService
    {

        public virtual (Type Type, string Path) Get(PropertyInfo property)
        {
            var isRootComponent = property.DeclaringType.Name.IsComponentVm();
            var componentType = property.DeclaringType;
            var path = string.Empty;

            if(isRootComponent)
            {
                path = property.Name.FirstCharacterToLower();
            }
            else
            {
                componentType = property.DeclaringType.GetComponent();
                List<string> paths = null;
                FindSubVmPropertyPath(componentType, new List<PropertyInfo>(), property, ref paths);

                path = string.Join(StringExtensions.PathDelimiter, paths);
            }

            path = string.Format("{0}{1}", StringExtensions.PathDelimiter, path);

            return (componentType, path);
        }

        public void FindSubVmPropertyPath(Type type, List<PropertyInfo> properties, PropertyInfo toFind, ref List<string> outputPath)
        {
            foreach(var p in type.GetProperties())
            {
                if(p == toFind)
                {
                    properties.Add(p);
                    outputPath = properties.Select(x => x.Name.FirstCharacterToLower()).ToList();
                }

                if(outputPath == null)
                {
                    if (p.PropertyType.Name.IsSubVm())
                    {
                        CallForSubVm(properties, p, p.PropertyType, toFind, ref outputPath);
                    }
                    if (p.PropertyType.IsGenericType && p.PropertyType.GetGenericArguments().FirstOrDefault().Name.IsSubVm())
                    {
                        var firstGenericType = p.PropertyType.GetGenericArguments().FirstOrDefault();
                        if(firstGenericType.Name.IsSubVm())
                        {
                            CallForSubVm(properties, p, firstGenericType, toFind, ref outputPath);
                        }
                    }
                }
            }
        }

        public void CallForSubVm(List<PropertyInfo> properties, PropertyInfo p, Type type, PropertyInfo toFind, ref List<string> outputPath)
        {
            var childProperties = new List<PropertyInfo>(properties);
            childProperties.Add(p);
            FindSubVmPropertyPath(type, childProperties, toFind, ref outputPath);
        }

    }
        
}
