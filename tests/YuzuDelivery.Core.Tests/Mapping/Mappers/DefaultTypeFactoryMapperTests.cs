using FluentAssertions;

namespace YuzuDelivery.Core.Mapping;

public class DefaultTypeFactoryMapperTests
{
    [Test]
    public void DefaultTypeFactoryMapper_WhenRegistered_AddsConfigForTypeFactoryRunner()
    {
        var builder = new MapperBuilder()
            .AddManualMapper(r =>
            {
                r.AddTypeFactoryWithContext<TestMappingContext, DestATypeFactory, DestA>();
            });

        builder.Build();

        var dest = builder.TypeFactoryRunner.Run<DestA>();
        dest.A.Should().Be(42);
    }


    class DestATypeFactory : IYuzuTypeFactory<DestA, TestMappingContext>
    {
        public DestA Create(TestMappingContext context)
        {
            return new DestA {A = 42};
        }
    }
}
