using Microsoft.Extensions.FileProviders;
using System.ComponentModel;

namespace YuzuDelivery.Core.Settings
{
    public class CoreSettings
    {
        private const string DefaultPartialPrefix = "par";
        private const string DefaultDataStructurePrefix = "data";
        private const string DefaultLayoutPrefix = "_";

        private const string DefaultSchemaMetaFileExtension = ".meta";
        private const string DefaultSchemaFileExtension = ".schema";

        private const string StaticSchemaPath = "./Yuzu/_templates/schema";
        private const string StaticConfigPath = "./Yuzu//YuzuConfig.json";


        [DefaultValue(DefaultPartialPrefix)]
        public string PartialPrefix { get; set; } = DefaultPartialPrefix;

        [DefaultValue(DefaultDataStructurePrefix)]
        public string DataStructurePrefix { get; set; } = DefaultDataStructurePrefix;

        [DefaultValue(DefaultLayoutPrefix)]
        public string LayoutPrefix { get; set; } = DefaultLayoutPrefix;



        [DefaultValue(DefaultSchemaMetaFileExtension)]
        public string SchemaMetaFileExtension { get; set; } = DefaultSchemaMetaFileExtension;

        [DefaultValue(DefaultSchemaFileExtension)]
        public string SchemaFileExtension { get; set; } = DefaultSchemaFileExtension;


        [DefaultValue(StaticSchemaPath)]
        public string SchemaPath { get; set; } = StaticSchemaPath;

        [DefaultValue(StaticConfigPath)]
        public string ConfigPath { get; set; } = StaticConfigPath;


        /// <summary>
        /// Only use to manuipulate template locations as part of the plugin development process
        /// </summary>
        public bool IsPluginDev { get; set; }

        public IFileProvider SchemaFileProvider { get; set; } = null!;
    }
}
