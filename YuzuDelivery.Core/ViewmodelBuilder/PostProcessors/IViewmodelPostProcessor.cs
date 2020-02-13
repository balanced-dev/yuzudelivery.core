using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YuzuDelivery.Core.ViewModelBuilder
{
    public interface IViewmodelPostProcessor
    {

        bool IsValid(string name);
        string Apply(string content);

    }
}
