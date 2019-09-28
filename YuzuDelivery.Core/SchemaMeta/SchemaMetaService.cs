using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace YuzuDelivery.Core
{
    public class SchemaMetaService : ISchemaMetaService
    {

        public virtual string Get(PropertyInfo property)
        {
            var isRootComponent = property.PropertyType.Name.IsComponentVm();
            var path = string.Empty;

            if(isRootComponent)
            {
                path = property.Name.FirstCharacterToLower();
            }
            else
            {
                var root = property.DeclaringType.GetComponent();
                List<string> paths = null;
                FindSubVmPropertyPath(root, new List<PropertyInfo>(), property, ref paths);

                path = string.Join(StringExtensions.PathDelimiter, paths);
            }
            return string.Format("{0}{1}", StringExtensions.PathDelimiter, path);
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
