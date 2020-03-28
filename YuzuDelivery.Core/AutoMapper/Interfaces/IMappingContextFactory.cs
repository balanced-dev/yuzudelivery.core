using System;
using System.Collections.Generic;
using System.Reflection;
using AutoMapper;

namespace YuzuDelivery.Core
{
    public interface IMappingContextFactory
    {
        T From<T>(ResolutionContext context)
            where T : YuzuMappingContext;
    }
}
