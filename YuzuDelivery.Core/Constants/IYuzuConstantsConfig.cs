using System;
using System.Reflection;
using System.Collections.Generic;

namespace YuzuDelivery.Core
{
    public interface IYuzuConstantsConfig
    {
        string BlockPrefix { get; set; }
        string SubPrefix { get; set; }
        string PagePrefix { get; set; }
        string BlockRefPrefix { get; set; }

        string TemplateFileExtension { get; set; }
    }
}