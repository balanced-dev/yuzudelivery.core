using System.Collections.Generic;
using NJsonSchema.CodeGeneration.CSharp;

namespace YuzuDelivery.Core.ViewmodelBuilder.NJsonSchema.CodeGeneration.Models;

public class YuzuCSharpGeneratorSettings : CSharpGeneratorSettings
{
    public List<string> AdditionalNamespaces { get; set; }
}
