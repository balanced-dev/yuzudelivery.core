using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.IO;
using YuzuDelivery.Core;

namespace YuzuDelivery.Core.ViewModelBuilder
{
    public class ReferencesService
    {
        public const string SchemaFileReferencePattern = @"""\$ref""\s?:\s?""\./(par|data).*schema""";
        public const string SchemaNameReferencePattern = @"""\$ref""\s?:\s?""\/(par|data).*\""";

        private string pagePath;
        private string blockPath;

        public ReferencesService(IYuzuConfiguration config)
        {
            pagePath = config.TemplateLocations.Where(x => x.Name == "Pages").Select(x => x.Schema).FirstOrDefault();
            blockPath = config.TemplateLocations.Where(x => x.Name == "Partials").Select(x => x.Schema).FirstOrDefault();
            pagePath = pagePath.EndsWith("/") ? pagePath : string.Format("{0}\\", pagePath);
            blockPath = blockPath.EndsWith("/") ? blockPath : string.Format("{0}\\", blockPath);
        }

        public void FixMultiple(ViewModelType viewModelType, int limit = 99999)
        {
            DirectoryInfo dir = new DirectoryInfo(GetDirectoryForType(viewModelType));
            var count = 0;

            foreach (var i in dir.GetFiles("*.schema"))
            {
                if (count == limit)
                    break;
                var file = ReadFile(i.FullName);
                file = Fix(file);
                WriteSchemaFile(i.FullName, file);
                count++;
            }
        }

        public string Fix(string file)
        {
            var matches = Regex.Matches(file, SchemaNameReferencePattern);

            foreach (Match match in matches)
            {
                var matchValue = match.Value
                    .Replace("\"$ref\" : \"", "")
                    .Replace("\"$ref\": \"", "")
                    .Replace("\"$ref\" :\"", "")
                    .Replace("\"$ref\":\"", "")
                    .Replace("\"", "");
                var replaceValue = string.Format(".{0}.schema\"", matchValue);

                var fixedMatches = Regex.Matches(file, SchemaFileReferencePattern.Replace("./(par|data).*schema", replaceValue));
                if (fixedMatches.Count == 0)
                {
                    var toReplace = string.Format("{0}\"", matchValue);
                    file = file.Replace(toReplace, replaceValue);
                }
            }
            return file;
        }

        private string ReadFile(string schemaFilename)
        {
            return File.ReadAllText(schemaFilename);
        }

        private void WriteSchemaFile(string schemaFileName, string content)
        {
            File.WriteAllText(schemaFileName, content);
        }

        private string GetDirectoryForType(ViewModelType viewModelType)
        {
            return viewModelType == ViewModelType.page ? pagePath : blockPath;
        }

    }
}
