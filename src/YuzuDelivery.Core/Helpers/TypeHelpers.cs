using System;
using YuzuDelivery.Core;

namespace YuzuDelivery.Core
{
    public static class TypeHelpers
    {
        public static string GetBlockName(this Type type, bool addFirstForwardSlash = false)
        {
            var output = type.Name.Replace(YuzuConstants.Configuration.BlockPrefix, "par");
            return addFirstForwardSlash ? $"/{output}" : output;
        }
        public static string GetModelName(this Type type)
        {
            return type.Name.Replace(YuzuConstants.Configuration.BlockPrefix, "");
        }
    }
}
