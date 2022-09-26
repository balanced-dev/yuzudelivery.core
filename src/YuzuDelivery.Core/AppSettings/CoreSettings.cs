using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace YuzuDelivery.Umbraco.Core
{
    public class CoreSettings
    {
        internal const string StaticPages = "/yuzu/_templates/src/pages";
        internal const string StaticPartials = "/yuzu/_templates/src/blocks";
        internal const string StaticSchemaMeta = "/yuzu/_templates/paths";

        [DefaultValue(StaticPages)]
        public string Pages { get; set; } = StaticPages;

        [DefaultValue(StaticPartials)]
        public string Partials { get; set; } = StaticPartials;

        [DefaultValue(StaticSchemaMeta)]
        public string SchemaMeta { get; set; } = StaticSchemaMeta;
    }
}
