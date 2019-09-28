using System.Reflection;

namespace YuzuDelivery.Core
{
    public interface ISchemaMetaService
    {
        string Get(PropertyInfo property);
    }
}