using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;

namespace YuzuDelivery.Umbraco.Blocks
{
    public class YuzuDefinitionTemplateSetup : IYuzuDefinitionTemplateSetup
    {
        private const string TemplatesNotFoundMessage = "Yuzu definition template directory {0} not found for location {1}";
        private const string TemplateLocationsNotSet = "Yuzu definition template locations not set";

        public IHandlebarsProvider hbsProvider;

        public YuzuDefinitionTemplateSetup(IHandlebarsProvider hbsProvider)
        {
            this.hbsProvider = hbsProvider;
        }

        public virtual Dictionary<string, Func<object, string>> RegisterAll()
        {
            if (Yuzu.Configuration.TemplateLocations == null || !Yuzu.Configuration.TemplateLocations.Any())
                throw new ArgumentNullException(TemplateLocationsNotSet);

            var templates = new Dictionary<string, Func<object, string>>();

            foreach (var location in Yuzu.Configuration.TemplateLocations)
            {
                TestDirectoryExists(location);
                var directory = GetDirectory(location);
                if (directory != null)
                {
                    ProcessTemplates(directory, ref templates, location);
                }
            }

            return templates;
        }

        public virtual void ProcessTemplates(DirectoryInfo directory, ref Dictionary<string, Func<object, string>> templates, ITemplateLocation location)
        {
            if (location.SearchSubDirectories)
                foreach (var d in directory.GetDirectories())
                {
                    ProcessTemplates(d, ref templates, location);
                }

            foreach (var f in directory.GetFiles().Where(x => x.Extension == Yuzu.Configuration.TemplateFileExtension))
            {
                if (location.RegisterAllAsPartials)
                {
                    RegisterPartial(f);
                }
                AddCompiledTemplates(f, ref templates);
            }
        }

        public virtual void TestDirectoryExists(ITemplateLocation location)
        {
            if (!Directory.Exists(location.Path)) throw new Exception(string.Format(TemplatesNotFoundMessage, location.Path, location.Name));
        }

        public virtual void RegisterPartial(FileInfo f)
        {
            using (var reader = new StringReader(GetFileText(f)))
            {
                var compiled = hbsProvider.Compile(reader);
                var templateName = Path.GetFileNameWithoutExtension(f.Name);

                hbsProvider.RegisterTemplate(templateName, compiled);
            }
        }

        public virtual void AddCompiledTemplates(FileInfo f, ref Dictionary<string, Func<object, string>> templates)
        {
            var source = GetFileText(f);
            var compiled = hbsProvider.Compile(source);
            templates.Add(Path.GetFileNameWithoutExtension(f.Name), compiled);
        }

        public virtual DirectoryInfo GetDirectory(ITemplateLocation location)
        {
            return new DirectoryInfo(location.Path);
        }

        public virtual string GetFileText(FileInfo file)
        {
            return System.IO.File.ReadAllText(file.FullName);
        }
    }

}
