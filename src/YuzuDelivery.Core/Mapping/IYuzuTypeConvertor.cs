namespace YuzuDelivery.Core.Mapping
{
    public interface IYuzuTypeConvertor { }

    public interface IYuzuTypeConvertor<in TSource, out TDest, in TContext>
        : IYuzuTypeConvertor
        where TContext : YuzuMappingContext
    {
        TDest Convert(TSource source, TContext context);
    }
}
