using System;
using System.Collections.Generic;
using System.IO;

namespace YuzuDelivery.Core
{
    public interface IYuzuDefinitionTemplateSetup
    {
        void AddCompiledTemplates(FileInfo f, ref Dictionary<string, Func<object, string>> templates);
        void ProcessTemplates(DirectoryInfo directory, ref Dictionary<string, Func<object, string>> templates, ITemplateLocation location);
        Dictionary<string, Func<object, string>> RegisterAll();
        void RegisterPartial(FileInfo f);
        void TestDirectoryExists(ITemplateLocation location);
    }
}