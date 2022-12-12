using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Reflection;

namespace YuzuDelivery.Core
{
    public class YuzuConstantsConfig : IYuzuConstantsConfig
    {
        public YuzuConstantsConfig()
        {
            BlockPrefix = "vmBlock_";
            SubPrefix = "vmSub_";
            PagePrefix = "vmPage_";
            BlockRefPrefix = "par";
            TemplateFileExtension = ".hbs";
        }

        public string BlockPrefix { get; set; }
        public string SubPrefix { get; set; }
        public string PagePrefix { get; set; }
        public string BlockRefPrefix { get; set; }
        public string TemplateFileExtension { get; set; }
    }

}
