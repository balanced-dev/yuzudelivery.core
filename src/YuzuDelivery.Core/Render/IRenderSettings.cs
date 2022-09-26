using System;

namespace YuzuDelivery.Core
{
    public interface IRenderSettings
    {
        int CacheExpiry { get; set; }
        string CacheName { get; set; }
        Func<object> Data { get; set; }
        bool ShowJson { get; set; }
        string Template { get; set; }
    }
}