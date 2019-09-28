using System.Reflection;

namespace YuzuDelivery.Core
{
    public interface ISchemaMetaPropertyService
    {
        string Get(PropertyInfo property);
    }
}