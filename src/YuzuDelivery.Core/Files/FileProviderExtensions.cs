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

        public static void GetPagesAndPartials(this IFileProvider contents, string extension, string partialPrefix, Action<bool, string, IFileInfo> action, string path = null)
        {
            if(path == null) path = string.Empty;

            foreach (var fileInfo in contents.GetDirectoryContents(path))
            {
                if (fileInfo.IsDirectory)
                {
                    contents.GetPagesAndPartials(extension, partialPrefix, action, Path.Combine(path, fileInfo.Name));
                }

                else if (Path.GetExtension(fileInfo.Name) == extension)
                {
                    var templateName = Path.GetFileNameWithoutExtension(fileInfo.Name);
                    var isPartial = fileInfo.Name.StartsWith(partialPrefix);
                    action(isPartial, templateName, fileInfo);
                }
            }
        }

    }
}
