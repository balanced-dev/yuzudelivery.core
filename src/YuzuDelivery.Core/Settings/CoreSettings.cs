using System.ComponentModel;

namespace YuzuDelivery.Core.Settings
{
    public class CoreSettings
    {
        private const string StaticPages = "./Yuzu/_templates/src/pages";
        private const string StaticPartials = "./Yuzu/_templates/src/blocks";
        private const string StaticConfigPath = "./Yuzu//YuzuConfig.json";

        [DefaultValue(StaticPages)]
        public string Pages { get; set; } = StaticPages;

        [DefaultValue(StaticPartials)]
        public string Partials { get; set; } = StaticPartials;

        [DefaultValue(StaticConfigPath)]
        public string ConfigPath { get; set; } = StaticConfigPath;
    }
}
