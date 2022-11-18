namespace YuzuDelivery.Core.Mapping
{
    public interface IYuzuTypeAfterConvertor {}

    public interface IYuzuTypeAfterConvertor<in TSource, in TDest, in TContext> : IYuzuTypeAfterConvertor
        where TContext : YuzuMappingContext
    {
        void Apply(TSource source, TDest dest, TContext context);
    }
}
