using FluentAssertions;

namespace YuzuDelivery.Core.Mapping;

public class DefaultTypeAfterMapperInheritanceTests
{

    [Test]
    public void DefaultTypeAfterMapper_AfterMapperConfiguredWithApplyToDerivedTypes_AppliesToDerivedTypes()
    {
        var mapper = new MapperBuilder()
                     .WithConfigureAction((_, cfg, _) =>
                     {
                         cfg.CreateMap<DownstreamSource, DownstreamDestination>();
                     })
                     .AddManualMapper(r =>
                     {
                         r.AddTypeAfterMapWithContext<TestMappingContext, MagicLinkAfterMapper>(applyToDerivedTypes: true);
                     })
                     .Build();

        var dest = mapper.Map<DownstreamDestination>(new DownstreamSource{ Heading = "Hello World" } );
        dest.Heading.Should().Be("Hello World");
        dest.MagicLink.Should().Be("https://hifi.agency");
    }

    interface ISource { }

    class Source : ISource
    {
        public string Heading { get; set; }
    }

    class DownstreamSource : Source { }

    class UpstreamDestination
    {
        public string Heading { get; set; }
        public string MagicLink { get; set; }
    }

    class DownstreamDestination : UpstreamDestination { }

    class MagicLinkAfterMapper : IYuzuTypeAfterConvertor<ISource, UpstreamDestination, TestMappingContext>
    {
        public void Apply(ISource source, UpstreamDestination dest, TestMappingContext context)
        {
            dest.MagicLink = "https://hifi.agency";
        }
    }
}
