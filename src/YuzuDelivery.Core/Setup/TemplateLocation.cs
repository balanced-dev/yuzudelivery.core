using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuzuDelivery.Core
{
    public class TemplateLocation : ITemplateLocation
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public string Schema { get; set; }
        public bool RegisterAllAsPartials { get; set; }
        public bool SearchSubDirectories { get; set; }
        public TemplateType TemplateType { get; set; }
    }

    public enum TemplateType
    {
        Page,
        Partial
    }
}
