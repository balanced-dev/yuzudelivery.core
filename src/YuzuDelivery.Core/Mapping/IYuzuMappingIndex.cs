using System;

namespace YuzuDelivery.Core.Mapping;

public interface IYuzuMappingIndex
{
    public Type GetViewModelType(Type cmsType);
}
