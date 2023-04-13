using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using FluentAssertions;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using YuzuDelivery.Core.Settings;

namespace YuzuDelivery.Core.Test
{
    public class SchemaMetaServiceTests
    {
        public ISchemaMetaPropertyService schemaMetaPropertyService;

        public SchemaMetaService svc;
        public YuzuConfiguration config;
        private IOptions<CoreSettings> coreSettings;

        private IFileProvider fileProvider;

        public string jsonPaths;
        public string jsonOfTypeParent;
        public string jsonOfTypeChild;

        [OneTimeSetUp]
        public void FixtureSetup()
        {
            YuzuConstants.Reset();
            YuzuConstants.Initialize(new YuzuConstantsConfig());
        }

        [SetUp]
        public void Setup()
        {
            schemaMetaPropertyService = Substitute.For<ISchemaMetaPropertyService>();
            config = new YuzuConfiguration();

            fileProvider = Substitute.For<IFileProvider>();

            coreSettings = Substitute.For<IOptions<CoreSettings>>();
            coreSettings.Value.Returns(Substitute.For<CoreSettings>());
            coreSettings.Value.SchemaFileProvider = fileProvider;

            svc = Substitute.ForPartsOf<SchemaMetaService>(schemaMetaPropertyService, Options.Create(config), coreSettings);

            jsonPaths = @"{
                'refs': {
                    '/contentRows': [
                        '/parTest', '/parTest2'
                    ]
                }
            }";

            jsonOfTypeParent = @"{
                'refs': {
                    '/content': [
                        '/dataGrid^parGridBuilder'
                    ],
                }
            }";

            jsonOfTypeChild = @"{
                'parGrid': {
                    'refs': {
                        '/rows/columns/items': [
                            '/parRte', '/parImage'
                        ]
                    }
                },
                'anyOfTypes': [
                    'parGrid'
                ]
            }";
        }

        [Test]
        public void GetPathSegments_PathPresentInSchemaMeta_ReturnsExpectedPathSegments()
        {
            var json = @"{
                'path': '/foo/bar/baz/baz.schema'
            }";

            var pathsJson = JObject.Parse(json);
            var vmType = typeof(vmBlock_Test);

            svc.Configure().GetPathFileData(vmType.Name).Returns(pathsJson);

            var output = svc.GetPathSegments(vmType.Name);
            output.Should().BeEquivalentTo("Foo", "Bar");
        }

        [Test]
        public void GetPathSegments_PathMissingFromSchemaMeta_ReturnsEmptyList()
        {
            var pathsJson = new JObject();
            var vmType = typeof(vmBlock_Test);

            svc.Configure().GetPathFileData(vmType.Name).Returns(pathsJson);

            var output = svc.GetPathSegments(vmType.Name);
            output.Should().BeEmpty();
        }

        [Test]
        public void Get_given_path_data_with_paths_at_property_then_convert_json_refs_to_vm_property_names()
        {
            var p = typeof(vmBlock_Test).GetProperty("ContentRows");
            var pathsJson = JObject.Parse(jsonPaths);

            schemaMetaPropertyService.Get(p).Returns((p.DeclaringType, "/contentRows"));
            svc.Configure().GetPathFileData(p.DeclaringType).Returns(pathsJson);

            var output = svc.Get(p, "refs");

            Assert.That(output[0], Is.EqualTo("vmBlock_Test"));
            Assert.That(output[1], Is.EqualTo("vmBlock_Test2"));
        }

        [Test]
        public void When_property_has_of_type_then_return_as_part_of_the_vm_name()
        {
            var p = typeof(vmBlock_Test).GetProperty("ContentGrid");
            var pathsJson = JObject.Parse(jsonOfTypeParent);

            svc.Configure().GetPathFileData(p.DeclaringType).Returns(pathsJson);

            var output = svc.Get(p.DeclaringType, "refs", "/content");

            Assert.That(output[0], Is.EqualTo("dataGrid^vmBlock_GridBuilder"));
        }

        [Test]
        public void Get_given_property_of_type_and_path_then_convert_json_refs_to_vm_property_names()
        {
            var p = typeof(vmBlock_Test).GetProperty("ContentGrid");
            var pathsJson = JObject.Parse(jsonOfTypeChild);

            svc.Configure().GetPathFileData(p.DeclaringType).Returns(pathsJson);

            var output = svc.Get(p.DeclaringType, "refs", "/rows/columns/items", "parGrid");

            Assert.That(output[0], Is.EqualTo("vmBlock_Rte"));
            Assert.That(output[1], Is.EqualTo("vmBlock_Image"));
        }

        [Test]
        public void Get_given_property_type_is_sub_then_use_root_component()
        {
            var p = typeof(vmBlock_Test).GetProperty("ContentGrid");
            var blockType = typeof(vmBlock_Test);
            var pathsJson = JObject.Parse(jsonOfTypeChild);

            config.ViewModels.Add(blockType);
            svc.Configure().GetPathFileData(blockType).Returns(pathsJson);

            var output = svc.Get(p.PropertyType, "refs", "/rows/columns/items", "parGrid");

            Assert.That(output[0], Is.EqualTo("vmBlock_Rte"));
            Assert.That(output[1], Is.EqualTo("vmBlock_Image"));
        }

        [Test]
        public void GetPathFileData_given_vm_property_when_paths_file_exists_then_return_parsed_path_file_object()
        {
               var p = typeof(vmBlock_Test).GetProperty("ContentRows");
            StubPathFile("vmBlock_Test", new Dictionary<string, string> { { "test.meta", jsonPaths } });

            var output = svc.GetPathFileData(p.DeclaringType);

            Assert.That(output, Is.EqualTo(JObject.Parse(jsonPaths)));
        }

        [Test]
        public void GetPathFileData_given_two_locations_when_paths_file_exists_in_second_location_then_return_parsed_path_file_object()
        {
            var p = typeof(vmBlock_Test).GetProperty("ContentRows");
            StubPathFile("vmBlock_Test", new Dictionary<string, string> { { "test.meta", jsonPaths } });

            var output = svc.GetPathFileData(p.DeclaringType);

            Assert.That(output, Is.EqualTo(JObject.Parse(jsonPaths)));
        }

        [Test]
        public void GetPathFileData_given_path_file_found_then_throw_exception()
        {
            var p = typeof(vmBlock_Test).GetProperty("ContentRows");

            var ex = Assert.Throws<Exception>(() => svc.GetPathFileData(p.DeclaringType));

            Assert.That(ex.Message == "Schema meta file not found for vmBlock_Test");
        }

        [Test]
        public void GetFileInfo_return_block_from_file_provider()
        {
            StubPathFile("vmBlock_Test", new Dictionary<string, string> { { "parTest.meta", jsonPaths } });

            var output = svc.GetFileInfo("vmBlock_Test");

            Assert.That(output.Name, Is.EqualTo(@"parTest.meta"));
        }

        [Test]
        public void GetFileInfo_return_page_from_file_provider()
        {
            StubPathFile("vmBlock_Test", new Dictionary<string, string> { { "test.meta", jsonPaths } });

            var output = svc.GetFileInfo("vmPage_Test");

            Assert.That(output.Name, Is.EqualTo(@"test.meta"));
        }

        [Test]
        public void GetFileInfo_return_block_first_from_file_provider()
        {
            StubPathFile("vmBlock_Test", new Dictionary<string, string> { { "test.meta", jsonPaths }, { "parTest.meta", jsonPaths } });

            var output = svc.GetFileInfo("vmBlock_Test");

            Assert.That(output.Name, Is.EqualTo(@"parTest.meta"));
        }

        public void StubPathFile(string declaringTypeName, Dictionary<string, string> files)
        {
            var dc = Substitute.For<IDirectoryContents>();
            var fileInfos = new List<IFileInfo>();
            foreach(var file in files)
            {
                var fileInfo = Substitute.For<IFileInfo>();
                fileInfo.Name.Returns(file.Key);
                var filestream = new MemoryStream(Encoding.UTF8.GetBytes(file.Value ?? ""));
                fileInfo.Configure().CreateReadStream().Returns(filestream);
                fileInfos.Add(fileInfo);
            }
            dc.Configure().GetEnumerator().Returns(fileInfos.GetEnumerator());
            fileProvider.Configure().GetDirectoryContents(String.Empty).Returns(dc);
        }

        public class vmBlock_Test
        {
            public string ContentRows { get; set; }
            public string ContentGrid { get; set; }
        }

    }
}
