using FluentAssertions;

namespace YuzuDelivery.Core.Mapping;

public class DefaultTypeReplaceMapperTests
{
    [Test]
    public void DefaultTypeReplaceMapper_WhenRegistered_TakesControlOfAllMapping()
    {
        var mapper = new MapperBuilder()
                     .AddManualMapper(r =>
                     {
                         r.AddTypeReplaceWithContext<TestMappingContext, SourceAToDestBTypeConverter>();
                     })
                     .Build();

        var dest = mapper.Map<DestB>(new SourceA {A = 42});
        dest.B.Should().Be(42);
    }

    [Test]
    public void DefaultTypeReplaceMapper_MultipleRegisted_LastRegisteredWins()
    {
        var mapper = new MapperBuilder()
                     .AddManualMapper(r =>
                     {
                         r.AddTypeReplaceWithContext<TestMappingContext, SourceAToDestBTypeConverter>();
                         r.AddTypeReplaceWithContext<TestMappingContext, SourceAToDestBDoublingTypeConverter>();
                     })
                     .Build();

        var dest = mapper.Map<DestB>(new SourceA {A = 42});
        dest.B.Should().Be(84);
    }



    class SourceAToDestBTypeConverter : IYuzuTypeConvertor<SourceA, DestB, TestMappingContext>
    {
        public DestB Convert(SourceA source, TestMappingContext context)
        {
            return new DestB
            {
                B = source.A
            };
        }
    }

    class SourceAToDestBDoublingTypeConverter : IYuzuTypeConvertor<SourceA, DestB, TestMappingContext>
    {
        public DestB Convert(SourceA source, TestMappingContext context)
        {
            return new DestB
            {
                B = source.A * 2
            };
        }
    }
}
