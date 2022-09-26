using System;
using System.Collections.Generic;
using System.Reflection;

namespace YuzuDelivery.Core
{
    public interface IYuzuBaseMapper
    {
        MethodInfo MakeGenericMethod(YuzuMapperSettings baseSettings);
    }
}
