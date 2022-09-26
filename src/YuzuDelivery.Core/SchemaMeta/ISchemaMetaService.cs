using System;
using System.Collections.Generic;
using System.Reflection;

namespace YuzuDelivery.Core
{
    public interface ISchemaMetaService
    {
        string GetOfType(PropertyInfo property, string area);
        string[] Get(Type propertyType, string area, string path, string ofType);
        string[] Get(Type propertyType, string area, string path);
        string[] Get(PropertyInfo property, string area);
    }
}