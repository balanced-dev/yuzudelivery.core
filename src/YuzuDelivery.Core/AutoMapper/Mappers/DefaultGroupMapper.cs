using AutoMapper;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace YuzuDelivery.Core
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class DefaultGroupMapper : IYuzuGroupMapper
    {
        public MethodInfo MakeGenericMethod(YuzuMapperSettings baseSettings)
        {
            if (baseSettings is not YuzuGroupMapperSettings settings)
            {
                throw new Exception($"Mapping settings not of type {nameof(YuzuGroupMapperSettings)}");
            }

            var genericArguments = new List<Type>
            {
                settings.Source,
                settings.DestParent,
                settings.DestChild
            };

            var method = GetType().GetMethod("CreateMap")!;
            return method.MakeGenericMethod(genericArguments.ToArray());
        }

        public AddedMapContext CreateMap<Source, DestParent, DestChild>(
            MapperConfigurationExpression cfg,
            YuzuMapperSettings baseSettings,
            IServiceProvider factory,
            AddedMapContext mapContext,
            IYuzuConfiguration config)
        {
            if (baseSettings is not YuzuGroupMapperSettings settings)
            {
                throw new Exception($"Mapping settings not of type {nameof(YuzuGroupMapperSettings)}");
            }

            var groupNameWithoutSpaces = settings.GroupName.Replace(" ", "");

            cfg.RecognizePrefixes(groupNameWithoutSpaces);

            mapContext.AddOrGet<Source, DestChild>(cfg);

            var parentMap = mapContext.AddOrGet<Source, DestParent>(cfg);
            parentMap.ForMember(settings.PropertyName, opt => opt.MapFrom(y => y));

            return mapContext;
        }
    }
}
