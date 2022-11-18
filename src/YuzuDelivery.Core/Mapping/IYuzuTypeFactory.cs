﻿namespace YuzuDelivery.Core.Mapping
{
    public interface IYuzuTypeFactory { }

    public interface IYuzuTypeFactory<out TDest, in TContext>
        : IYuzuTypeFactory
        where TContext : YuzuMappingContext
    {
        TDest Create(TContext context);
    }
}