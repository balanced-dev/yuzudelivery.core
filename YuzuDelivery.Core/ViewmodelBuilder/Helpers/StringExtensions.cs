using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YuzuDelivery.Core.ViewModelBuilder
{
    public static class StringExtensions
    {

        public static string FirstLetterToUpperCase(this string s)
        {
            if (string.IsNullOrEmpty(s))
                throw new ArgumentException("There is no first letter");

            char[] a = s.ToCharArray();
            a[0] = char.ToUpper(a[0]);
            return new string(a);
        }

        public static string SchemaIdToName(this string s)
        {
            if (string.IsNullOrEmpty(s))
                throw new ArgumentException("String is null");

            if (s.Contains("/par"))
                return s.Replace("/par", "");
            else
                return s.Replace("/", "").FirstLetterToUpperCase();
        }

        public static string CleanRefName(this string s)
        {
            if (string.IsNullOrEmpty(s))
                throw new ArgumentException("String is null");

            return s.Replace("./par", "");
        }

        public static string RemoveFileSuffix(this string s)
        {
            if (string.IsNullOrEmpty(s))
                throw new ArgumentException("String is null");

            if (s.StartsWith("par"))
                return s.Substring(3, s.Length - 3);
            else
                return s.FirstLetterToUpperCase();
        }

        public static string CleanFileExtension(this string s)
        {
            if (string.IsNullOrEmpty(s))
                throw new ArgumentException("String is null");

            return s.Split('.').FirstOrDefault();
        }

        public static string RemoveVmTypePrefix(this string s)
        {
            s = s.Replace("vmPage_", "");
            s = s.Replace("vmBlock_", "");
            s = s.Replace("vmSub_", "");
            return s;
        }

        public static string AddVmTypePrefix(this string s, ViewModelType viewModelType)
        {
            if (string.IsNullOrEmpty(s))
                throw new ArgumentException("String is null");

            if(viewModelType == ViewModelType.block)
                return string.Format("vmBlock_{0}", s);
            else
                return string.Format("vmPage_{0}", s);
        }

        public static string AddSubVmTypePrefixGenerated(this string parentTypeName, int? index = null)
        {
            if (string.IsNullOrEmpty(parentTypeName))
                throw new ArgumentException("String is null");

            if(index == null)
                return string.Format("vmSub_{0}_", parentTypeName);
            else
                return string.Format("vmSub_{0}_{1}", parentTypeName, index + 1);
        }

        public static string AddSubVmTypePrefix(this string parentTypeName, string typeName, bool removeLastUnderscore = false)
        {
            if (string.IsNullOrEmpty(parentTypeName))
                throw new ArgumentException("String is null");

            if(!removeLastUnderscore)
                return string.Format("vmSub_{0}_{1}", parentTypeName, typeName);
            else
                return string.Format("vmSub_{0}{1}", parentTypeName, typeName);
        }

    }
}
