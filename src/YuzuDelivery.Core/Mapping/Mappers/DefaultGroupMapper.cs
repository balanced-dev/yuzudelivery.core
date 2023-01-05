using System;
using System.Collections.Generic;
using System.Reflection;
using AutoMapper;
using YuzuDelivery.Core.Mapping.Mappers.Settings;

namespace YuzuDelivery.Core.Mapping.Mappers
{
    public interface IYuzuGroupMapper : IYuzuBaseMapper
    {
        void CreateMap<TSource, TParent, TChild>(
            MapperConfigurationExpression cfg,
            YuzuGroupMapperSettings settings,
            IServiceProvider factory,
            AddedMapContext mapContext,
            YuzuConfiguration config);
    }

    public class DefaultGroupMapper : YuzuBaseMapper<YuzuGroupMapperSettings>, IYuzuGroupMapper
    {
        public void CreateMap<Source, DestParent, DestChild>(
            MapperConfigurationExpression cfg,
            YuzuGroupMapperSettings settings,
            IServiceProvider factory,
            AddedMapContext mapContext,
            YuzuConfiguration config)
        {
            var groupNameWithoutSpaces = settings.GroupName.Replace(" ", "");

            cfg.RecognizePrefixes(groupNameWithoutSpaces);

            mapContext.AddOrGet<Source, DestChild>(cfg);

            var parentMap = mapContext.AddOrGet<Source, DestParent>(cfg);
            parentMap.ForMember(settings.PropertyName, opt => opt.MapFrom(y => y));
        }

        protected override MethodInfo MakeGenericMethod(YuzuGroupMapperSettings settings)
        {
            var genericArguments = new List<Type>
            {
                settings.Source,
                settings.DestParent,
                settings.DestChild
            };

            var method = GetType().GetMethod(nameof(CreateMap))!;
            return method.MakeGenericMethod(genericArguments.ToArray());
        }
    }
}
