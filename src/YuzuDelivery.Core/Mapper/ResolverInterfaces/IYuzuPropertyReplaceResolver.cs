namespace YuzuDelivery.Core
{
    public interface IYuzuPropertyReplaceResolver { }

    public interface IYuzuPropertyReplaceResolver<in TSource, out TDest, in TContext>
        : IYuzuPropertyReplaceResolver
        where TContext : YuzuMappingContext
    {
        TDest Resolve(TSource source, TContext context);
    }
}
