using System;
using System.Collections.Generic;
using System.Reflection;
using AutoMapper;
using YuzuDelivery.Core.AutoMapper.Mappers.Settings;

namespace YuzuDelivery.Core.AutoMapper.Mappers
{
    public interface IYuzuGroupMapper : IYuzuBaseMapper
    {
        void CreateMap<Model, VParent, VChild>(MapperConfigurationExpression cfg, YuzuMapperSettings baseSettings, IServiceProvider factory, AddedMapContext mapContext, IYuzuConfiguration config);
    }

    // ReSharper disable once ClassNeverInstantiated.Global
    public class DefaultGroupMapper : IYuzuGroupMapper
    {
        public void CreateMapAbstraction(
            MapperConfigurationExpression cfg,
            YuzuMapperSettings baseSettings,
            IServiceProvider factory,
            AddedMapContext mapContext,
            IYuzuConfiguration config)
        {
            var method = MakeGenericMethod(baseSettings);
            method.Invoke(this, new object[] {cfg, baseSettings, factory, mapContext, config});
        }

        public void CreateMap<Source, DestParent, DestChild>(
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
        }


        private MethodInfo MakeGenericMethod(YuzuMapperSettings baseSettings)
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
    }
}
