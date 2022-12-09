using FluentAssertions;

namespace YuzuDelivery.Core.Mapping;

public class DefaultTypeAfterMapperTests
{

    [Test]
    public void DefaultTypeAfterMapper_WhenRegistered_FirstAddsConventionBasedMapping()
    {
        var mapper = new MapperBuilder()
                     .AddManualMapper(r =>
                     {
                         r.AddTypeAfterMapWithContext<TestMappingContext, NoOpAfterConverter>();
                     })
                     .Build();

        var dest = mapper.Map<DestA>(new SourceA {A = 2});
        dest.A.Should().Be(2);
    }

    [Test]
    public void DefaultTypeAfterMapper_SingleMapperRegistered_AppliesAfterConventionMapping()
    {
        var mapper = new MapperBuilder()
                     .AddManualMapper(r =>
                     {
                         r.AddTypeAfterMapWithContext<TestMappingContext, AddTwoAfterConverter>();
                     })
                     .Build();

        var dest = mapper.Map<DestA>(new SourceA {A = 2});
        dest.A.Should().Be(4);
    }

    [Test]
    public void DefaultTypeAfterMapper_MultipleRegistered_BothRun()
    {
        var mapper = new MapperBuilder()
                     .AddManualMapper(r =>
                     {
                         r.AddTypeAfterMapWithContext<TestMappingContext, AddTwoAfterConverter>();
                         r.AddTypeAfterMapWithContext<TestMappingContext, NegatingAfterConverter>();
                     })
                     .Build();

        var dest = mapper.Map<DestA>(new SourceA {A = 2});
        dest.A.Should().Be(-4);
    }

    [Test]
    public void DefaultTypeAfterMapper_MultipleRegistered_RespectRegistrationOrder()
    {
        var mapper = new MapperBuilder()
                     .AddManualMapper(r =>
                     {
                         r.AddTypeAfterMapWithContext<TestMappingContext, NegatingAfterConverter>();
                         r.AddTypeAfterMapWithContext<TestMappingContext, AddTwoAfterConverter>();
                     })
                     .Build();

        var dest = mapper.Map<DestA>(new SourceA {A = 2});
        dest.A.Should().Be(0);
    }

    class NoOpAfterConverter : IYuzuTypeAfterConvertor<SourceA, DestA, TestMappingContext>
    {
        public void Apply(SourceA source, DestA dest, TestMappingContext context)
        {
            // NOOP
        }
    }

    class AddTwoAfterConverter : IYuzuTypeAfterConvertor<SourceA, DestA, TestMappingContext>
    {
        public void Apply(SourceA source, DestA dest, TestMappingContext context)
        {
            dest.A += 2;
        }
    }

    class NegatingAfterConverter : IYuzuTypeAfterConvertor<SourceA, DestA, TestMappingContext>
    {
        public void Apply(SourceA source, DestA dest, TestMappingContext context)
        {
            dest.A = -dest.A;
        }
    }
}
