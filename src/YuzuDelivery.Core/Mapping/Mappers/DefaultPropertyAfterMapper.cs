using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using YuzuDelivery.Core.Mapping.Mappers.Settings;

namespace YuzuDelivery.Core.Mapping.Mappers
{
    public interface IYuzuPropertyAfterMapper : IYuzuBaseMapper
    {
        void CreateMap<TSource, TMember, TDest, TService>(
            MapperConfigurationExpression cfg,
            YuzuPropertyAfterMapperSettings settings,
            IServiceProvider factory,
            AddedMapContext mapContext,
            IYuzuConfiguration config)
            where TService : class, IYuzuPropertyAfterResolver<TMember>;
    }

    public interface IYuzuPropertyAfterResolver<TMember> : IYuzuPropertyAfterResolver, IYuzuMappingResolver
    {
        TMember Apply(TMember value);
    }

    public class DefaultPropertyAfterMapper : YuzuBaseMapper<YuzuPropertyAfterMapperSettings>,  IYuzuPropertyAfterMapper
    {
        public void CreateMap<TSource, TMember, TDest, TService>(
            MapperConfigurationExpression cfg,
            YuzuPropertyAfterMapperSettings settings,
            IServiceProvider factory,
            AddedMapContext mapContext,
            IYuzuConfiguration config)
            where TService : class, IYuzuPropertyAfterResolver<TMember>
        {

            //need a fix here
            //config.AddActiveManualMap<Resolver, Dest>(settings.DestProperty);

            if (!string.IsNullOrEmpty(settings.GroupName))
            {
                cfg.RecognizePrefixes(settings.GroupName);
            }

            Func<TMember, TMember> mappingFunction = (input) =>
            {
                var propertyResolver = factory.GetRequiredService<TService>();
                return propertyResolver.Apply(input);
            };

            var map = mapContext.AddOrGet<TSource, TDest>(cfg);

            map.ForMember(settings.DestProperty as Expression<Func<TDest, TMember>>, opt => {
                opt.AddTransform(x => mappingFunction(x));
            });
        }

        protected override MethodInfo MakeGenericMethod(YuzuPropertyAfterMapperSettings settings)
        {
            var genericArguments = settings.Resolver.GetRelatedTypeParameters();
            genericArguments.Add(settings.Dest);
            genericArguments.Add(settings.Resolver);

            var method = GetType().GetMethod(nameof(CreateMap))!;
            return method.MakeGenericMethod(genericArguments.ToArray());
        }
    }
}
