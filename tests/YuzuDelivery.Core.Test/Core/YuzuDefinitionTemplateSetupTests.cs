using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using YuzuDelivery.Core;
using System.IO;

namespace YuzuDelivery.Core.Test
{
    [TestFixture, Ignore("Directory Info is a sealed class and not mackable")]
    public class YuzuDefinitionTemplateSetupTests
    {
        private YuzuDefinitionTemplateSetup svc;

        private IHandlebarsProvider hbs;
        private IYuzuConstantsConfig constantsConfig;
        private IYuzuConfiguration config;

        private List<ITemplateLocation> templateLocations;
        private Dictionary<string, Func<object, string>> templates;

        private DirectoryInfo directory;
        private DirectoryInfo subdirectory;

        private List<FileInfo> files;
        private string fileName = "testFile";
        private string fileContent = string.Empty;
        private string fileExtension = ".hbs";

        [OneTimeSetUp]
        public void FixtureSetup()
        {
            constantsConfig = Substitute.For<IYuzuConstantsConfig>();
            YuzuConstants.Reset();
            YuzuConstants.Initialize(constantsConfig);
        }

        [SetUp]
        public void Setup()
        {
            hbs = Substitute.For<IHandlebarsProvider>();
            config = Substitute.For<IYuzuConfiguration>();

            templates = new Dictionary<string, Func<object, string>>();
            templateLocations = new List<ITemplateLocation>();

            directory = Substitute.For<DirectoryInfo>();
            subdirectory = Substitute.For<DirectoryInfo>();

            files = new List<FileInfo>();

            directory.GetDirectories().Returns(new DirectoryInfo[] { subdirectory });

            config.TemplateLocations = templateLocations;
            constantsConfig.TemplateFileExtension = fileExtension;

            svc = Substitute.ForPartsOf<YuzuDefinitionTemplateSetup>(hbs, config);
        }

        #region register_all

        [Test]
        public void given_template_locations_not_set_then_throw_excpetion()
        {
            config.TemplateLocations = null;
            svc.RegisterAll();

            Assert.Throws<Exception>(() => svc.RegisterAll());
        }

        [Test]
        public void given_template_locations_not_present_then_throw_excpetion()
        {
            Assert.Throws<Exception>(() => svc.RegisterAll());
        }

        [Test]
        public void given_template_locations_then_process_single_location()
        {
            var location = StubLocationForRegister();

            svc.RegisterAll();

            svc.Received().ProcessTemplates(directory, ref templates, location);
        }

        [Test]
        public void given_template_locations_then_process_multiple_location()
        {
            var location = StubLocationForRegister();
            var location2 = StubLocationForRegister();

            svc.RegisterAll();

            svc.Received().ProcessTemplates(directory, ref templates, location);
            svc.Received().ProcessTemplates(directory, ref templates, location2);
        }

        [Test]
        public void given_template_locations_where_directory_unavailable_then_dont_process()
        {
            var location = StubLocationForRegister(false);

            svc.RegisterAll();

            svc.Received().ProcessTemplates(directory, ref templates, location);
        }

        public ITemplateLocation StubLocationForRegister(bool hasDirectory = true)
        {
            var location = new TemplateLocation();
            templateLocations.Add(location);

            svc.TestDirectoryExists(location);
            svc.ProcessTemplates(directory, ref templates, location);

            if (hasDirectory)
                svc.GetDirectory(location).Returns(directory);
            else
                svc.GetDirectory(location).Returns((DirectoryInfo) null);

            return location;
        }

        #endregion

        #region process_templates

        [Test]
        public void given_file_with_correct_extension_then_compile_template()
        {
            var location = StubLocationForProcess();
            var file = StubFile();

            StubDirectoryFiles();

            svc.AddCompiledTemplates(file, ref templates);

            svc.ProcessTemplates(directory, ref templates, location);

            svc.Received().AddCompiledTemplates(file, ref templates);
        }

        [Test]
        public void given_files_with_correct_extension_then_compile_templates()
        {
            var location = StubLocationForProcess();
            var file = StubFile();
            var file2 = StubFile();

            StubDirectoryFiles();

            svc.AddCompiledTemplates(file, ref templates);
            svc.AddCompiledTemplates(file2, ref templates);

            svc.ProcessTemplates(directory, ref templates, location);

            svc.Received().AddCompiledTemplates(file, ref templates);
            svc.Received().AddCompiledTemplates(file2, ref templates);
        }

        [Test]
        public void given_file_with_incorrect_extension_then_dont_compile()
        {
            var location = StubLocationForProcess();
            var file = StubFile("jpg");

            StubDirectoryFiles();

            svc.ProcessTemplates(directory, ref templates, location);

            svc.Received().AddCompiledTemplates(file, ref templates);
        }

        [Test]
        public void given_location_has_partials_then_add_file_as_partial()
        {
            var location = StubLocationForProcess(false, true);
            var file = StubFile();

            StubDirectoryFiles();

            svc.AddCompiledTemplates(file, ref templates);
            svc.RegisterPartial(file);

            svc.ProcessTemplates(directory, ref templates, location);

            svc.Received().RegisterPartial(file);
        }

        [Test]
        public void given_location_includes_subdirectories_then_process_subdirectories()
        {
            var location = StubLocationForProcess(true, false);
            var file = StubFile();

            StubDirectoryFiles();

            svc.AddCompiledTemplates(file, ref templates);
            svc.ProcessTemplates(subdirectory, ref templates, location);

            svc.ProcessTemplates(directory, ref templates, location);

            svc.Received().ProcessTemplates(subdirectory, ref templates, location);
        }

        public FileInfo StubFile(string ext = ".hbs")
        {
            var file = Substitute.For<FileInfo>();
            file.Extension.Returns(ext);
            file.Name.Returns(string.Format("{0}{1}", fileName, ext));
            files.Add(file);

            return file;
        }

        public ITemplateLocation StubLocationForProcess(bool searchSubDirectories = false, bool registerAllAsPartials = false)
        {
            var location = new TemplateLocation();
            location.SearchSubDirectories = searchSubDirectories;
            location.RegisterAllAsPartials = registerAllAsPartials;
            return location;
        }

        public void StubDirectoryFiles()
        {
            directory.GetFiles().Returns(files.ToArray());
        }

        #endregion


        #region register_partial

        [Test]
        public void given_file_then_get_content_compile_them_and_register_as_a_template()
        {
            var file = StubFile();

            Action<TextWriter, object> compiled = (TextWriter txt, object obj) => { };

            svc.GetFileText(file).Returns(fileContent);
            hbs.Compile(Arg.Any<TextReader>()).Returns(compiled);

            svc.RegisterPartial(file);

            hbs.Received().RegisterTemplate(fileName, compiled);
        }

        [Test]
        public void given_file_then_add_the_compiled_template_to_templates_store()
        {
            var file = StubFile();

            Func<object, string> compiled = (object obj) => { return null; };

            svc.GetFileText(file).Returns(fileContent);
            hbs.Compile(fileContent).Returns(compiled);

            svc.AddCompiledTemplates(file, ref templates);

            Assert.IsTrue(templates.ContainsKey(fileName));
            Assert.AreEqual(templates[fileName], compiled);
        }

        #endregion
    }
}
