using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NJsonSchema;
using NJsonSchema.CodeGeneration;

namespace YuzuDelivery.Core.ViewModelBuilder
{
    public class ViewModelPropertyNameGenerator : IPropertyNameGenerator
    {
        public string Generate(JsonSchemaProperty property)
        {
            return property.Name.FirstLetterToUpperCase();
        }
    }
}
