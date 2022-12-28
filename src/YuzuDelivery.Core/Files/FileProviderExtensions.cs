using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YuzuDelivery.Core.Settings;

namespace YuzuDelivery.Core
{
    public static class FileProviderExtensions
    {

        public static void GetPagesAndPartials(this IFileProvider contents, string extension, CoreSettings setting, Action<bool, bool, string, IFileInfo> action, string path = null)
        {
            if(path == null) path = string.Empty;

            foreach (var fileInfo in contents.GetDirectoryContents(path))
            {
                if (fileInfo.IsDirectory)
                {
                    contents.GetPagesAndPartials(extension, setting, action, Path.Combine(path, fileInfo.Name));
                }

                else if (Path.GetExtension(fileInfo.Name) == extension)
                {
                    var templateName = Path.GetFileNameWithoutExtension(fileInfo.Name);
                    var isPartial = fileInfo.Name.StartsWith(setting.PartialPrefix) || fileInfo.Name.StartsWith(setting.DataStructurePrefix);
                    var isLayout = fileInfo.Name.StartsWith(setting.LayoutPrefix);
                    action(isPartial, isLayout, templateName, fileInfo);
                }
            }
        }

    }
}
