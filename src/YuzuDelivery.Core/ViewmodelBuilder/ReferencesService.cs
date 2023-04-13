using System.Text.RegularExpressions;

namespace YuzuDelivery.Core.ViewModelBuilder
{
    public class ReferencesService
    {
        public const string SchemaFileReferencePattern = @"""\$ref""\s?:\s?""\./(par|data).*schema""";
        public const string SchemaNameReferencePattern = @"""\$ref""\s?:\s?""\/(par|data).*\""";

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

    }
}
