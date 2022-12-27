using Microsoft.Extensions.FileProviders;
using System.ComponentModel;

namespace YuzuDelivery.Core.Settings
{
    public class CoreSettings
    {
        private const string StaticSchema = "./Yuzu/_templates/schema";
        private const string StaticConfigPath = "./Yuzu//YuzuConfig.json";

        [DefaultValue(StaticSchema)]
        public string Schema { get; set; } = StaticSchema;

        [DefaultValue(StaticConfigPath)]
        public string ConfigPath { get; set; } = StaticConfigPath;

        /// <summary>
        /// Only use to manuipulate template locations as part of the plugin development process
        /// </summary>
        public bool IsPluginDev { get; set; }

        public IFileProvider SchemaFileProvider { get; set; } = null!;
    }
}
