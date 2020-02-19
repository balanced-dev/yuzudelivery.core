using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NJsonSchema;
using NJsonSchema.CodeGeneration;
using Inflector;

namespace YuzuDelivery.Core.ViewModelBuilder
{
    public class ViewModelTypeNameGenerator : ITypeNameGenerator
    {
        public string Generate(
                JsonSchema schema,
                string typeNameHint,
                IEnumerable<string> reservedTypeNames)
        {
            var parentObjectName = GetCurrentSchemaId(schema);
            var objectName = string.Empty;
            var parentSchema = schema.Parent as JsonSchema;

            //root node
            if (parentObjectName == typeNameHint)
                objectName = typeNameHint.AddVmTypePrefix(SchemaName.ViewModelType);
            //external schema
            else if (typeNameHint != null && (typeNameHint.StartsWith("Par") || typeNameHint.StartsWith("Data")))
            {
                objectName = schema.Id.SchemaIdToName().AddVmTypePrefix(ViewModelType.block);
            }
            //external schema in array
            else if (typeNameHint != null && parentSchema != null && parentSchema.Item != null && parentSchema.Item.Properties.Any(x => x.Key.ToLower() == typeNameHint.ToLower()))
            {
                var refSchema = parentSchema.Item.Properties.Where(x => x.Key.ToLower() == typeNameHint.ToLower()).Select(x => x.Value).FirstOrDefault();
                if (refSchema.ActualSchema != null)
                    objectName = refSchema.ActualSchema.Id.SchemaIdToName().AddVmTypePrefix(ViewModelType.block);
            }
            //external schema sub schema, ignore
            else if (SchemaName.RootSchema != parentObjectName)
            {
                objectName = "DoNotApply";
            }
            else if (SchemaName.RootSchema == parentObjectName && typeNameHint == null)
            {
                objectName = parentObjectName.AddVmTypePrefix(ViewModelType.block);
            }
            //tablehint doesn't exist create index from parent object
            else if (string.IsNullOrEmpty(typeNameHint))
            {
                var rootGenName = parentObjectName.AddSubVmTypePrefixGenerated();
                var index = reservedTypeNames.Where(x => x.StartsWith(rootGenName)).Count();
                objectName = parentObjectName.AddSubVmTypePrefixGenerated(index);
            }
            //subVm has a name use it to create the type name
            else
            {
                typeNameHint = typeNameHint.FirstLetterToUpperCase();
                if (schema.Parent != null && schema.Parent is JsonSchemaProperty)
                {
                    var parentPropertySchema = schema.Parent as JsonSchemaProperty;
                    if(parentPropertySchema.IsArray)
                        objectName = parentObjectName.AddSubVmTypePrefix(typeNameHint.Singularize(), true);
                }
                
                if(string.IsNullOrEmpty(objectName))
                {
                    objectName = parentObjectName.AddSubVmTypePrefix(typeNameHint, true);
                }
            }

            return objectName;
        }

        protected string GetCurrentSchemaId(JsonSchema schema)
        {
            //this is the root not of current content
            if (schema.Id != null)
            {
                var currentSchema = schema.Id.SchemaIdToName();
                if (SchemaName.RootSchema == currentSchema)
                    return currentSchema;
            }

            //works out the parent schema of child schemas, even from the root of an external schema
            while (schema.Parent != null)
            {
                schema = schema.Parent as JsonSchema;
                if (schema.Id != null)
                    return schema.Id.SchemaIdToName();
            }
            return string.Empty;
        }
    }
}
