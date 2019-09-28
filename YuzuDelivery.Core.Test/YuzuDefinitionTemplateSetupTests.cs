using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Rhino.Mocks;
using YuzuDelivery.Core;
using System.IO;

namespace YuzuDelivery.Core.Test
{
    [TestFixture]
    public class YuzuDefinitionTemplateSetupTests
    {
        private YuzuDefinitionTemplateSetup svc;

        private IHandlebarsProvider hbs;
        private IYuzuConfiguration config;

        private List<ITemplateLocation> templateLocations;
        private Dictionary<string, Func<object, string>> templates;

        private DirectoryInfo directory;
        private DirectoryInfo subdirectory;

        private List<FileInfo> files;
        private string fileName = "testFile";
        private string fileContent = string.Empty;
        private string fileExtension = ".hbs";

        [TestFixtureSetUp]
        public void FixtureSetup()
        {
            config = MockRepository.GenerateStub<IYuzuConfiguration>();
            Yuzu.Reset();
            Yuzu.Initialize(config);
        }

        [SetUp]
        public void Setup()
        {
            hbs = MockRepository.GenerateStub<IHandlebarsProvider>();

            templates = new Dictionary<string, Func<object, string>>();
            templateLocations = new List<ITemplateLocation>();

            directory = MockRepository.GenerateStub<DirectoryInfo>();
            subdirectory = MockRepository.GenerateStub<DirectoryInfo>();

            files = new List<FileInfo>();

            directory.Stub(x => x.GetDirectories()).Return(new DirectoryInfo[] { subdirectory });

            Yuzu.Configuration.TemplateLocations = templateLocations;
            Yuzu.Configuration.TemplateFileExtension = fileExtension;

            svc = MockRepository.GeneratePartialMock<YuzuDefinitionTemplateSetup>(new object[] { hbs });
        }

        #region register_all

        [Test, ExpectedException()]
        public void given_template_locations_not_set_then_throw_excpetion()
        {
            Yuzu.Configuration.TemplateLocations = null;
            svc.RegisterAll();
        }

        [Test, ExpectedException()]
        public void given_template_locations_not_present_then_throw_excpetion()
        {
            svc.RegisterAll();
        }

        [Test]
        public void given_template_locations_then_process_single_location()
        {
            var location = StubLocationForRegister();

            svc.RegisterAll();

            svc.AssertWasCalled(x => x.ProcessTemplates(directory, ref templates, location));
        }

        [Test]
        public void given_template_locations_then_process_multiple_location()
        {
            var location = StubLocationForRegister();
            var location2 = StubLocationForRegister();

            svc.RegisterAll();

            svc.AssertWasCalled(x => x.ProcessTemplates(directory, ref templates, location));
            svc.AssertWasCalled(x => x.ProcessTemplates(directory, ref templates, location2));
        }

        [Test]
        public void given_template_locations_where_directory_unavailable_then_dont_process()
        {
            var location = StubLocationForRegister(false);

            svc.RegisterAll();

            svc.AssertWasNotCalled(x => x.ProcessTemplates(directory, ref templates, location));
        }

        public ITemplateLocation StubLocationForRegister(bool hasDirectory = true)
        {
            var location = new TemplateLocation();
            templateLocations.Add(location);

            svc.Stub(x => x.TestDirectoryExists(location));
            svc.Stub(x => x.ProcessTemplates(directory, ref templates, location));

            if (hasDirectory)
                svc.Stub(x => x.GetDirectory(location)).Return(directory);
            else
                svc.Stub(x => x.GetDirectory(location)).Return(null);

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

            svc.Stub(x => x.AddCompiledTemplates(file, ref templates));

            svc.ProcessTemplates(directory, ref templates, location);

            svc.AssertWasCalled(x => x.AddCompiledTemplates(file, ref templates));
        }

        [Test]
        public void given_files_with_correct_extension_then_compile_templates()
        {
            var location = StubLocationForProcess();
            var file = StubFile();
            var file2 = StubFile();

            StubDirectoryFiles();

            svc.Stub(x => x.AddCompiledTemplates(file, ref templates));
            svc.Stub(x => x.AddCompiledTemplates(file2, ref templates));

            svc.ProcessTemplates(directory, ref templates, location);

            svc.AssertWasCalled(x => x.AddCompiledTemplates(file, ref templates));
            svc.AssertWasCalled(x => x.AddCompiledTemplates(file2, ref templates));
        }

        [Test]
        public void given_file_with_incorrect_extension_then_dont_compile()
        {
            var location = StubLocationForProcess();
            var file = StubFile("jpg");

            StubDirectoryFiles();

            svc.ProcessTemplates(directory, ref templates, location);

            svc.AssertWasNotCalled(x => x.AddCompiledTemplates(file, ref templates));
        }

        [Test]
        public void given_location_has_partials_then_add_file_as_partial()
        {
            var location = StubLocationForProcess(false, true);
            var file = StubFile();

            StubDirectoryFiles();

            svc.Stub(x => x.AddCompiledTemplates(file, ref templates));
            svc.Stub(x => x.RegisterPartial(file));

            svc.ProcessTemplates(directory, ref templates, location);

            svc.AssertWasCalled(x => x.RegisterPartial(file));
        }

        [Test]
        public void given_location_includes_subdirectories_then_process_subdirectories()
        {
            var location = StubLocationForProcess(true, false);
            var file = StubFile();

            StubDirectoryFiles();

            svc.Stub(x => x.AddCompiledTemplates(file, ref templates));
            svc.Stub(x => x.ProcessTemplates(subdirectory, ref templates, location));

            svc.ProcessTemplates(directory, ref templates, location);

            svc.AssertWasCalled(x => x.ProcessTemplates(subdirectory, ref templates, location));
        }

        public FileInfo StubFile(string ext = ".hbs")
        {
            var file = MockRepository.GenerateStub<FileInfo>();
            file.Stub(x => x.Extension).Return(ext);
            file.Stub(x => x.Name).Return(string.Format("{0}{1}", fileName, ext));
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
            directory.Stub(x => x.GetFiles()).Return(files.ToArray());
        }

        #endregion


        #region register_partial

        [Test]
        public void given_file_then_get_content_compile_them_and_register_as_a_template()
        {
            var file = StubFile();

            Action<TextWriter, object> compiled = (TextWriter txt, object obj) => { };

            svc.Stub(x => x.GetFileText(file)).Return(fileContent);
            hbs.Stub(x => x.Compile(Arg<TextReader>.Is.Anything)).Return(compiled);

            svc.RegisterPartial(file);

            hbs.AssertWasCalled(x => x.RegisterTemplate(fileName, compiled));
        }

        [Test]
        public void given_file_then_add_the_compiled_template_to_templates_store()
        {
            var file = StubFile();

            Func<object, string> compiled = (object obj) => { return null; };

            svc.Stub(x => x.GetFileText(file)).Return(fileContent);
            hbs.Stub(x => x.Compile(fileContent)).Return(compiled);

            svc.AddCompiledTemplates(file, ref templates);

            Assert.IsTrue(templates.ContainsKey(fileName));
            Assert.AreEqual(templates[fileName], compiled);
        }

        #endregion
    }
}
