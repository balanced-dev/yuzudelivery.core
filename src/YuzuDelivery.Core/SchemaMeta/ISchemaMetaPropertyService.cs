using System.Reflection;
using System;

namespace YuzuDelivery.Core
{
    public interface ISchemaMetaPropertyService
    {
        (Type Type, string Path) Get(PropertyInfo property);
    }
}