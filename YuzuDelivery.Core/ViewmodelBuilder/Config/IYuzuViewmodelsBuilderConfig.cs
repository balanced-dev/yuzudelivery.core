
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YuzuDelivery.Core.ViewModelBuilder
{
    public interface IYuzuViewmodelsBuilderConfig
    {
        bool EnableViewmodelsBuilder { get; set; }
        string GeneratedViewmodelsNamespace { get; set; }
        List<string> ExcludeViewmodelsAtGeneration { get; set; }
        List<string> AddNamespacesAtGeneration { get; set; }
        string GeneratedViewmodelsOutputFolder { get; set; }
    }

    public interface IUpdateableVmBuilderConfig
    {
        List<string> ExcludeViewmodelsAtGeneration { get; set; }
        List<string> AddNamespacesAtGeneration { get; set; }
    }
}
