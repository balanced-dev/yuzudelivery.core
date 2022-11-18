namespace YuzuDelivery.Core.Mapping
{
    public interface IYuzuPropertyReplaceResolver { }

    public interface IYuzuPropertyReplaceResolver<in TSource, out TDest, in TContext>
        : IYuzuPropertyReplaceResolver
        where TContext : YuzuMappingContext
    {
        TDest Resolve(TSource source, TContext context);
    }
}
