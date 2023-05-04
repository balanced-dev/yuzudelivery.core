using System;
using System.Collections.Generic;

namespace YuzuDelivery.Core.Mapping;

public class BadConfigurationYuzuMapper : IMapper
{
    private readonly YuzuMapperConfigurationException _ex;

    public BadConfigurationYuzuMapper(Exception ex)
    {
        _ex = new YuzuMapperConfigurationException(ex);
    }

    public object Concrete => _ex;
    public TDest Map<TDest>(object source) => throw _ex;

    public TDest Map<TSource, TDest>(TSource source) => throw _ex;
    public TDest Map<TDest>(object source, IDictionary<string, object> items) => throw _ex;
    public TDest Map<TSource, TDest>(TSource source, IDictionary<string, object> items) => throw _ex;
    public object Map(object source, Type sourceType, Type destinationType, IDictionary<string, object> items) => throw _ex;

    // ReSharper disable once MemberCanBePrivate.Global
    public class YuzuMapperConfigurationException : ApplicationException
    {
        private const string DefaultMessage = "Failed to construct a mapper instance due to an invalid expression in mapping configuration.";

        public YuzuMapperConfigurationException(Exception ex)
            : base(DefaultMessage, ex)
        { }
    }
}
