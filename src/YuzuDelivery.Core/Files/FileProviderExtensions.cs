using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YuzuDelivery.Core
{
    public static class FileProviderExtensions
    {

        public static void GetPagesAndPartials(this IFileProvider contents, string extension, string[] partialPrefixes, string[] layoutPrefixes, Action<bool, bool, string, IFileInfo> action, string path = null)
        {
            if(path == null) path = string.Empty;

            foreach (var fileInfo in contents.GetDirectoryContents(path))
            {
                if (fileInfo.IsDirectory)
                {
                    contents.GetPagesAndPartials(extension, partialPrefixes, layoutPrefixes, action, Path.Combine(path, fileInfo.Name));
                }

                else if (Path.GetExtension(fileInfo.Name) == extension)
                {
                    var templateName = Path.GetFileNameWithoutExtension(fileInfo.Name);
                    var isPartial = partialPrefixes.Any(x => fileInfo.Name.StartsWith(x));
                    var isLayout = layoutPrefixes.Any(x => fileInfo.Name.StartsWith(x));
                    action(isPartial, isLayout, templateName, fileInfo);
                }
            }
        }

    }
}
