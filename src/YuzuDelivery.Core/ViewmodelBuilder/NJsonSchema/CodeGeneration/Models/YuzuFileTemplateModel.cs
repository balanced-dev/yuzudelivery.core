using System.Collections.Generic;
using NJsonSchema.CodeGeneration.CSharp.Models;

namespace YuzuDelivery.Core.ViewmodelBuilder.NJsonSchema.CodeGeneration.Models;

public class YuzuFileTemplateModel : FileTemplateModel
{
    public List<string> AdditionalNamespaces { get; set; }
}
