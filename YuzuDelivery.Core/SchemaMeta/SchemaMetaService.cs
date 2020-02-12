using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace YuzuDelivery.Core
{
    public class SchemaMetaService : ISchemaMetaService
    {
        protected ISchemaMetaPropertyService schemaMetaPropertyService;
        protected readonly IYuzuConfiguration config;

        public SchemaMetaService(ISchemaMetaPropertyService schemaMetaPropertyService, IYuzuConfiguration config)
        {
            this.schemaMetaPropertyService = schemaMetaPropertyService;
            this.config = config;
        }

        public virtual string GetOfType(PropertyInfo property, string area)
        {
            var types = Get(property, area);
            var firstType = types.FirstOrDefault();
            if (firstType != null && firstType.Contains("^"))
            {
                return firstType.Split('^').ElementAt(1);
            }
            else
                return string.Empty;
        }

        public virtual string[] Get(Type propertyType, string area, string path, string ofType)
        {
            var pathData = GetPathFileData(propertyType.GetComponent(config));

            if (pathData[ofType] != null && pathData[ofType][area] != null && pathData[ofType][area][path] != null)
            {
                var allowedTypes = pathData[ofType][area][path].ToObject<string[]>();
                return allowedTypes.Select(x => x.BlockRefToVmTypeName()).ToArray();
            }
            else
                return new string[] { };
        }

        public virtual string[] Get(Type propertyType, string area, string path)
        {
            var pathData = GetPathFileData(propertyType.GetComponent(config));

            if (pathData[area] != null && pathData[area][path] != null)
            {
                var allowedTypes = pathData[area][path].ToObject<string[]>();
                return allowedTypes.Select(x => x.BlockRefToVmTypeName()).ToArray();
            }
            else
                return new string[] { };
        }

        public virtual string[] Get(PropertyInfo property, string area)
        {
            var component = schemaMetaPropertyService.Get(property);
            var pathData = GetPathFileData(component.Type);

            if (pathData[area] != null && pathData[area][component.Path] != null)
            {
                var allowedTypes = pathData[area][component.Path].ToObject<string[]>();
                return allowedTypes.Select(x => x.BlockRefToVmTypeName()).ToArray();
            }
            else
                return new string[] { };
        }

        public virtual JObject GetPathFileData(Type propertyType)
        {
            var typeName = propertyType.Name;
            var isPage = typeName.IsPage();
            string pathFilename = string.Empty;

            //get paths file from frontend solution
            foreach(var location in config.SchemaMetaLocations) { 
                var filename = GetPathFileName(location.Path, typeName, isPage);
                if (FileExists(filename))
                    pathFilename = filename;
            };

            if (pathFilename != string.Empty)
                return JsonConvert.DeserializeObject<JObject>(FileRead(pathFilename));
            else
                throw new Exception(string.Format("Paths file not found for {0}", typeName));
        }

        public virtual string GetPathFileName(string rootPath, string declaringTypeName, bool IsPage)
        {
            string filePatttern = IsPage ? "{0}/{1}.schema" : "{0}/{2}{1}.schema";

            return string.Format(filePatttern, rootPath, declaringTypeName.RemoveAllVmPrefixes(), YuzuConstants.Configuration.BlockRefPrefix.RemoveFirstForwardSlash());
        }

        public virtual bool FileExists(string pathFilename)
        {
            return System.IO.File.Exists(pathFilename);
        }

        public virtual string FileRead(string pathFilename)
        {
            return System.IO.File.ReadAllText(pathFilename);
        }
    }
        
}
