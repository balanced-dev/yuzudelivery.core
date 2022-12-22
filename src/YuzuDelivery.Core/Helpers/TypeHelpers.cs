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

        public static string GetTemplateName(this Type vmType)
        {
            var suspectName = vmType.Name.RemoveAllVmPrefixes();

            // allow getting template name from base view models
            if (vmType.BaseType?.Name?.StartsWith("vm") ?? false)
            {
                suspectName = vmType.BaseType.Name.RemoveAllVmPrefixes();
            }

            if (vmType.Name.IsPage())
            {
                return suspectName.FirstCharacterToLower();
            }

            return $"par{suspectName}";
        }
    }
}
