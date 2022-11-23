using FluentAssertions;

namespace YuzuDelivery.Core.Mapping;

public class DefaultYuzuMapperTests
{
    [Test]
    public void DefaultYuzuMapper_WithRegisteredTypeReplaceMapper_MapsAsExpected()
    {
        var mapper = new MapperBuilder()
                     .AddTypeReplaceMapper<SourceConverter>()
                     .Build();

        var dest = mapper.Map<Dest>(new Source {Input = "Foo"});
        dest.Result.Should().Be("Foo");
    }

    class Source
    {
        public string Input { get; set; }
    }

    class Dest
    {
        public string Result { get; set; }
    }

    class SourceConverter : IYuzuTypeConvertor<Source, Dest, TestMappingContext>
    {
        public Dest Convert(Source source, TestMappingContext context)
        {
            return new Dest
            {
                Result = source.Input
            };
        }
    }
}
