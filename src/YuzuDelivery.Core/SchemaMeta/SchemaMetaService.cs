using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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
            var component = schemaMetaPropertyService.Get(property);
            var pathData = GetPathFileData(component.Type);
            var types = new string[] { };

            if (pathData[area] != null && pathData[area][component.Path] != null)
            {
                var allowedTypes = pathData[area][component.Path].ToObject<string[]>();
                //don't convert this to vmBlock notation
                types = allowedTypes.Select(x => x).ToArray();
            }

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

        public virtual string GetString(Type propertyType, string area, string path)
        {
            var pathData = GetPathFileData(propertyType.GetComponent(config));

            if (pathData[area] != null && pathData[area][path] != null)
            {
                return pathData[area][path].ToString();
            }
            else
                return null;
        }

        public string[] GetPathSegments(Type vmType)
        {
            var meta = GetPathFileData(vmType);
            if (!meta.ContainsKey("path"))
            {
                throw new InvalidOperationException($"Schema meta for type: {vmType.FullName} has no path configured");
            }
            var path = meta["path"].ToString();

            return path
                   .TrimStart('/')
                   .Split('/')
                   .SkipLast(2)
                   .Select(x => x.Replace("_", ""))
                   .Select(StringExtensions.FirstCharacterToUpper)
                   .ToArray();
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

            //get paths file from frontend solution

            foreach (var schemaMetaFile in GetPossiblePathFileName(typeName))
            {
                if (FileExists(schemaMetaFile))
                {
                    return JsonConvert.DeserializeObject<JObject>(FileRead(schemaMetaFile));
                }
            };

            throw new Exception(string.Format("Schema meta file not found for {0}", typeName));
        }

        public virtual IEnumerable<string> GetPossiblePathFileName(string declaringTypeName)
        {
            var typeNameNoPrefix = declaringTypeName.RemoveAllVmPrefixes();
            var blockPrefix = YuzuConstants.Configuration.BlockRefPrefix.RemoveFirstForwardSlash();

            foreach (var templateLocation in config.TemplateLocations)
            {
                yield return Path.Combine(templateLocation.Schema, $"{blockPrefix}{typeNameNoPrefix}.meta"); // blocks prioritized over pages
                yield return Path.Combine(templateLocation.Schema, $"{typeNameNoPrefix.FirstCharacterToLower()}.meta");
            }
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
