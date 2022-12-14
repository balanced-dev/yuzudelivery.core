using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using System.Reflection;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using YuzuDelivery.Core;

namespace YuzuDelivery.Core.Test
{
    public class SchemaMetaServiceTests
    {
        public ISchemaMetaPropertyService schemaMetaPropertyService;

        public SchemaMetaService svc;
        public IYuzuConfiguration config;

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
            config = Substitute.For<IYuzuConfiguration>();

            svc = Substitute.ForPartsOf<SchemaMetaService>(schemaMetaPropertyService, config);

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

            StubConfigViewmodels(blockType);
            svc.Configure().GetPathFileData(blockType).Returns(pathsJson);

            var output = svc.Get(p.PropertyType, "refs", "/rows/columns/items", "parGrid");

            Assert.That(output[0], Is.EqualTo("vmBlock_Rte"));
            Assert.That(output[1], Is.EqualTo("vmBlock_Image"));
        }

        [Test]
        public void GetPathFileData_given_vm_property_when_paths_file_exists_then_return_parsed_path_file_object()
        {
            config.TemplateLocations.Returns(new List<ITemplateLocation>
            {
                new TemplateLocation
                {
                    Schema = "c:/test"
                }
            });

            var p = typeof(vmBlock_Test).GetProperty("ContentRows");
            CreatePathDataLocations(new string[] { "c:/test" });
            StubPathFile("c:/test", "vmBlock_Test", "path");

            var output = svc.GetPathFileData(p.DeclaringType);

            Assert.That(output, Is.EqualTo(JObject.Parse(jsonPaths)));
        }

        [Test]
        public void GetPathFileData_given_two_locations_when_paths_file_exists_in_second_location_then_return_parsed_path_file_object()
        {
            config.TemplateLocations.Returns(new List<ITemplateLocation>
            {
                new TemplateLocation
                {
                    Schema = "c:/test"
                },
                new TemplateLocation
                {
                    Schema = "c:/test2"
                }
            });

            var p = typeof(vmBlock_Test).GetProperty("ContentRows");
            CreatePathDataLocations(new string[] { "c:/test", "c:/test2" });
            StubPathFile("c:/test", "vmBlock_Test", "not-here", false);
            StubPathFile("c:/test2", "vmBlock_Test", "path");

            var output = svc.GetPathFileData(p.DeclaringType);

            Assert.That(output, Is.EqualTo(JObject.Parse(jsonPaths)));
        }

        [Test]
        public void GetPathFileData_given_path_file_found_then_throw_exception()
        {
            var p = typeof(vmBlock_Test).GetProperty("ContentRows");

            config.TemplateLocations.Returns(new List<ITemplateLocation>());

            var ex = Assert.Throws<Exception>(() => svc.GetPathFileData(p.DeclaringType));

            Assert.That(ex.Message == "Schema meta file not found for vmBlock_Test");
        }

        [Test]
        public void GetPossiblePathFileName_return_possible_paths_with_par_prefix_and_without_prefix()
        {

            if(RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                config.TemplateLocations.Returns(new List<ITemplateLocation>
                {
                    new TemplateLocation
                    {
                        Schema = @"c:\test\"
                    }
                });
                var output = svc.GetPossiblePathFileName("vmPage_Test");

                Assert.That(output.First(), Is.EqualTo(@"c:\test\parTest.meta"));
                Assert.That(output.Last(), Is.EqualTo(@"c:\test\test.meta"));
            }
            else
            {
                config.TemplateLocations.Returns(new List<ITemplateLocation>
                {
                    new TemplateLocation
                    {
                        Schema = @"/foo/test/"
                    }
                });
                var output = svc.GetPossiblePathFileName("vmPage_Test");

                Assert.That(output.First(), Is.EqualTo("/foo/test/parTest.meta"));
                Assert.That(output.Last(), Is.EqualTo("/foo/test/test.meta"));
            }
        }

        public void CreatePathDataLocations(string[] locations)
        {
            var schemaMetaLocations = locations.Select(x => new DataLocation() { Path = x }).Cast<IDataLocation>().ToList();
        }

        public void StubPathFile(string rootPath, string declaringTypeName, string filePath, bool exists = true)
        {
            svc.Configure().GetPossiblePathFileName(declaringTypeName).Returns(new string[] { filePath });
            svc.Configure().FileExists(filePath).Returns(exists);
            svc.Configure().FileRead(filePath).Returns(jsonPaths);
        }

        public void StubConfigViewmodels(Type type)
        {
            var viewmodels = new List<Type>();
            viewmodels.Add(type);

            config.ViewModels.Returns(viewmodels);
        }

        public class vmBlock_Test
        {
            public string ContentRows { get; set; }
            public string ContentGrid { get; set; }
        }

    }
}
