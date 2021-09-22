using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Moq;
using YuzuDelivery.Core;
using System.IO;

namespace YuzuDelivery.Core.Test
{
    [TestFixture, Ignore("Properties not supported by Moq")]
    public class YuzuDefinitionTemplateSetupTests
    {
        private Mock<YuzuDefinitionTemplateSetup> svc;

        private Mock<IHandlebarsProvider> hbs;
        private Mock<IYuzuConstantsConfig> constantsConfig;
        private Mock<IYuzuConfiguration> config;

        private List<ITemplateLocation> templateLocations;
        private Dictionary<string, Func<object, string>> templates;

        private Mock<DirectoryInfo> directory;
        private Mock<DirectoryInfo> subdirectory;

        private List<FileInfo> files;
        private string fileName = "testFile";
        private string fileContent = string.Empty;
        private string fileExtension = ".hbs";

        [OneTimeSetUp]
        public void FixtureSetup()
        {
            constantsConfig = new Moq.Mock<IYuzuConstantsConfig>();
            YuzuConstants.Reset();
            YuzuConstants.Initialize(constantsConfig.Object);
        }

        [SetUp]
        public void Setup()
        {
            hbs = new Moq.Mock<IHandlebarsProvider>();
            config = new Moq.Mock<IYuzuConfiguration>();

            templates = new Dictionary<string, Func<object, string>>();
            templateLocations = new List<ITemplateLocation>();

            directory = new Moq.Mock<DirectoryInfo>();
            subdirectory = new Moq.Mock<DirectoryInfo>();

            files = new List<FileInfo>();

            directory.Setup(x => x.GetDirectories()).Returns(new DirectoryInfo[] { subdirectory.Object });

            config.Object.TemplateLocations = templateLocations;
            constantsConfig.Object.TemplateFileExtension = fileExtension;

            svc = new Moq.Mock<YuzuDefinitionTemplateSetup>(MockBehavior.Loose, hbs, config) { CallBase = true };
        }

        #region register_all

        [Test]
        public void given_template_locations_not_set_then_throw_excpetion()
        {
            config.Object.TemplateLocations = null;
            svc.Object.RegisterAll();

            Assert.Throws<Exception>(() => svc.Object.RegisterAll());
        }

        [Test]
        public void given_template_locations_not_present_then_throw_excpetion()
        {
            Assert.Throws<Exception>(() => svc.Object.RegisterAll());
        }

        [Test]
        public void given_template_locations_then_process_single_location()
        {
            var location = StubLocationForRegister();

            svc.Object.RegisterAll();

            svc.Verify(x => x.ProcessTemplates(directory.Object, ref templates, location));
        }

        [Test]
        public void given_template_locations_then_process_multiple_location()
        {
            var location = StubLocationForRegister();
            var location2 = StubLocationForRegister();

            svc.Object.RegisterAll();

            svc.Verify(x => x.ProcessTemplates(directory.Object, ref templates, location));
            svc.Verify(x => x.ProcessTemplates(directory.Object, ref templates, location2));
        }

        [Test]
        public void given_template_locations_where_directory_unavailable_then_dont_process()
        {
            var location = StubLocationForRegister(false);

            svc.Object.RegisterAll();

            svc.Verify(x => x.ProcessTemplates(directory.Object, ref templates, location));
        }

        public ITemplateLocation StubLocationForRegister(bool hasDirectory = true)
        {
            var location = new TemplateLocation();
            templateLocations.Add(location);

            svc.Setup(x => x.TestDirectoryExists(location));
            svc.Setup(x => x.ProcessTemplates(directory.Object, ref templates, location));

            if (hasDirectory)
                svc.Setup(x => x.GetDirectory(location)).Returns(directory.Object);
            else
                svc.Setup(x => x.GetDirectory(location)).Returns((DirectoryInfo) null);

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

            svc.Setup(x => x.AddCompiledTemplates(file, ref templates));

            svc.Object.ProcessTemplates(directory.Object, ref templates, location);

            svc.Verify(x => x.AddCompiledTemplates(file, ref templates));
        }

        [Test]
        public void given_files_with_correct_extension_then_compile_templates()
        {
            var location = StubLocationForProcess();
            var file = StubFile();
            var file2 = StubFile();

            StubDirectoryFiles();

            svc.Setup(x => x.AddCompiledTemplates(file, ref templates));
            svc.Setup(x => x.AddCompiledTemplates(file2, ref templates));

            svc.Object.ProcessTemplates(directory.Object, ref templates, location);

            svc.Verify(x => x.AddCompiledTemplates(file, ref templates));
            svc.Verify(x => x.AddCompiledTemplates(file2, ref templates));
        }

        [Test]
        public void given_file_with_incorrect_extension_then_dont_compile()
        {
            var location = StubLocationForProcess();
            var file = StubFile("jpg");

            StubDirectoryFiles();

            svc.Object.ProcessTemplates(directory.Object, ref templates, location);

            svc.Verify(x => x.AddCompiledTemplates(file, ref templates));
        }

        [Test]
        public void given_location_has_partials_then_add_file_as_partial()
        {
            var location = StubLocationForProcess(false, true);
            var file = StubFile();

            StubDirectoryFiles();

            svc.Setup(x => x.AddCompiledTemplates(file, ref templates));
            svc.Setup(x => x.RegisterPartial(file));

            svc.Object.ProcessTemplates(directory.Object, ref templates, location);

            svc.Verify(x => x.RegisterPartial(file));
        }

        [Test]
        public void given_location_includes_subdirectories_then_process_subdirectories()
        {
            var location = StubLocationForProcess(true, false);
            var file = StubFile();

            StubDirectoryFiles();

            svc.Setup(x => x.AddCompiledTemplates(file, ref templates));
            svc.Setup(x => x.ProcessTemplates(subdirectory.Object, ref templates, location));

            svc.Object.ProcessTemplates(directory.Object, ref templates, location);

            svc.Verify(x => x.ProcessTemplates(subdirectory.Object, ref templates, location));
        }

        public FileInfo StubFile(string ext = ".hbs")
        {
            var file = new Moq.Mock<FileInfo>();
            file.Setup(x => x.Extension).Returns(ext);
            file.Setup(x => x.Name).Returns(string.Format("{0}{1}", fileName, ext));
            files.Add(file.Object);

            return file.Object;
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
            directory.Setup(x => x.GetFiles()).Returns(files.ToArray());
        }

        #endregion


        #region register_partial

        [Test]
        public void given_file_then_get_content_compile_them_and_register_as_a_template()
        {
            var file = StubFile();

            Action<TextWriter, object> compiled = (TextWriter txt, object obj) => { };

            svc.Setup(x => x.GetFileText(file)).Returns(fileContent);
            hbs.Setup(x => x.Compile(It.IsAny<TextReader>())).Returns(compiled);

            svc.Object.RegisterPartial(file);

            hbs.Verify(x => x.RegisterTemplate(fileName, compiled));
        }

        [Test]
        public void given_file_then_add_the_compiled_template_to_templates_store()
        {
            var file = StubFile();

            Func<object, string> compiled = (object obj) => { return null; };

            svc.Setup(x => x.GetFileText(file)).Returns(fileContent);
            hbs.Setup(x => x.Compile(fileContent)).Returns(compiled);

            svc.Object.AddCompiledTemplates(file, ref templates);

            Assert.IsTrue(templates.ContainsKey(fileName));
            Assert.AreEqual(templates[fileName], compiled);
        }

        #endregion
    }
}
