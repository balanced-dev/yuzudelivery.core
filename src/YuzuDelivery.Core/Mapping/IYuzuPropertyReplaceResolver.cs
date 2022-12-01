namespace YuzuDelivery.Core.Mapping
{
    public interface IYuzuPropertyReplaceResolver { }

    public interface IYuzuPropertyReplaceResolver<in TSource, out TMember, in TContext>
        : IYuzuPropertyReplaceResolver
        where TContext : YuzuMappingContext
    {
        TMember Resolve(TSource source, TContext context);
    }
}
