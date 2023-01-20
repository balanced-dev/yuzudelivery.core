using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using YuzuDelivery.Core.Settings;

namespace YuzuDelivery.Core
{
    public class SchemaMetaService : ISchemaMetaService
    {
        protected ISchemaMetaPropertyService schemaMetaPropertyService;
        private IOptions<YuzuConfiguration> config;
        private readonly CoreSettings coreSettings;
        private readonly IFileProvider fileProvider;

        public SchemaMetaService(ISchemaMetaPropertyService schemaMetaPropertyService,  IOptions<YuzuConfiguration> config, IOptions<CoreSettings> coreSettings)
        {
            this.schemaMetaPropertyService = schemaMetaPropertyService;
            this.config = config;
            this.coreSettings = coreSettings.Value;
            this.fileProvider = coreSettings.Value.SchemaFileProvider;
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
            var pathData = GetPathFileData(propertyType.GetComponent(config.Value));

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
            var pathData = GetPathFileData(propertyType.GetComponent(config.Value));

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
            var pathData = GetPathFileData(propertyType.GetComponent(config.Value));

            if (pathData[area] != null && pathData[area][path] != null)
            {
                return pathData[area][path].ToString();
            }
            else
                return null;
        }

        public string[] GetPathSegments(string viewModelName)
        {
            var meta = GetPathFileData(viewModelName);
            if (!meta.ContainsKey("path"))
            {
                throw new InvalidOperationException($"Schema meta for viewModel: '{viewModelName}' has no path configured");
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

        public bool TryGetPathSegments(string viewModelName, out string[] result)
        {
            try
            {
                result = GetPathSegments(viewModelName);
                return true;
            }
            catch (Exception e)
            {
                result = null;
                return false;
            }
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
            return GetPathFileData(propertyType.Name);
        }

        public virtual JObject GetPathFileData(string propertyType)
        {
            var fileInfo = GetFileInfo(propertyType);
            if(fileInfo != null)
            {
                var fileStream = fileInfo.CreateReadStream();
                using var reader = new StreamReader(fileStream);
                return JsonConvert.DeserializeObject<JObject>(reader.ReadToEnd());
            }
            else
                throw new Exception(string.Format("Schema meta file not found for {0}", propertyType));
        }

        public virtual IFileInfo GetFileInfo(string declaringTypeName)
        {
            var files = new List<IFileInfo>();
            fileProvider.GetPagesAndPartials(coreSettings.SchemaMetaFileExtension, coreSettings,
                (bool isPartial, bool isLayout, string name, IFileInfo fileInfo) => {
                    if (isPartial) files.Add(fileInfo);
                    if (!isLayout && !isPartial) files.Add(fileInfo);
                });

            var typeNameNoPrefix = declaringTypeName.RemoveAllVmPrefixes();
            var blockPrefix = YuzuConstants.Configuration.BlockRefPrefix.RemoveFirstForwardSlash();

            var fromBlock = files.FirstOrDefault(x => x.Name == $"{blockPrefix}{typeNameNoPrefix}.meta");
            if (fromBlock != null) return fromBlock;

            return files.FirstOrDefault(x => x.Name == $"{typeNameNoPrefix.FirstCharacterToLower()}.meta");
        }
    }

}
