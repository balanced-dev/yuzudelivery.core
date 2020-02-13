
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YuzuDelivery.Core.ViewModelBuilder
{
    public interface IUpdateableVmBuilderConfig
    {
        List<string> ExcludeViewmodelsAtGeneration { get; set; }
        List<string> AddNamespacesAtGeneration { get; set; }
    }
}
