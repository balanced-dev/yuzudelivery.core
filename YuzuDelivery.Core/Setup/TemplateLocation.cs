using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuzuDelivery.Umbraco.Blocks
{
    public class TemplateLocation : ITemplateLocation
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public bool RegisterAllAsPartials { get; set; }
        public bool SearchSubDirectories { get; set; }
    }
}
