﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Moq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using YuzuDelivery.Core;

namespace YuzuDelivery.Core.Test
{
    public class SchemaMetaServiceTests
    {
        public Mock<ISchemaMetaPropertyService> schemaMetaPropertyService;

        public Mock<SchemaMetaService> svc;
        public Mock<IYuzuConfiguration> config;

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
            schemaMetaPropertyService = new Moq.Mock<ISchemaMetaPropertyService>();
            config = new Moq.Mock<IYuzuConfiguration>();
            config.Object.SchemaMetaLocations = new List<IDataLocation>();

            svc = new Moq.Mock<SchemaMetaService>(MockBehavior.Loose, schemaMetaPropertyService.Object, config.Object) { CallBase = true };

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
                        '/parDataGridRows^parGrid'
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

            schemaMetaPropertyService.Setup(x => x.Get(p)).Returns((p.DeclaringType, "/contentRows"));
            svc.Setup(x => x.GetPathFileData(p.DeclaringType)).Returns(pathsJson);

            var output = svc.Object.Get(p, "refs");

            Assert.AreEqual("vmBlock_Test", output[0]);
            Assert.AreEqual("vmBlock_Test2", output[1]);
        }

        [Test]
        public void When_property_has_of_type_then_return_as_part_of_the_vm_name()
        {
            var p = typeof(vmBlock_Test).GetProperty("ContentGrid");
            var pathsJson = JObject.Parse(jsonOfTypeParent);

            svc.Setup(x => x.GetPathFileData(p.DeclaringType)).Returns(pathsJson);

            var output = svc.Object.Get(p.DeclaringType, "refs", "/content");

            Assert.AreEqual("vmBlock_DataGridRows^parGrid", output[0]);
        }

        [Test]
        public void Get_given_property_of_type_and_path_then_convert_json_refs_to_vm_property_names()
        {
            var p = typeof(vmBlock_Test).GetProperty("ContentGrid");
            var pathsJson = JObject.Parse(jsonOfTypeChild);

            svc.Setup(x => x.GetPathFileData(p.DeclaringType)).Returns(pathsJson);

            var output = svc.Object.Get(p.DeclaringType, "refs", "/rows/columns/items", "parGrid");

            Assert.AreEqual("vmBlock_Rte", output[0]);
            Assert.AreEqual("vmBlock_Image", output[1]);
        }

        [Test]
        public void Get_given_property_type_is_sub_then_use_root_component()
        {
            var p = typeof(vmBlock_Test).GetProperty("ContentGrid");
            var blockType = typeof(vmBlock_Test);
            var pathsJson = JObject.Parse(jsonOfTypeChild);

            StubConfigViewmodels(blockType);
            svc.Setup(x => x.GetPathFileData(blockType)).Returns(pathsJson);

            var output = svc.Object.Get(p.PropertyType, "refs", "/rows/columns/items", "parGrid");

            Assert.AreEqual("vmBlock_Rte", output[0]);
            Assert.AreEqual("vmBlock_Image", output[1]);
        }

        [Test]
        public void GetPathFileData_given_vm_property_when_paths_file_exists_then_return_parsed_path_file_object()
        {
            var p = typeof(vmBlock_Test).GetProperty("ContentRows");
            CreatePathDataLocations(new string[] { "c:/test" });
            StubPathFile("c:/test", "vmBlock_Test", "path");

            var output = svc.Object.GetPathFileData(p.DeclaringType);

            Assert.AreEqual(JObject.Parse(jsonPaths), output);
        }

        [Test]
        public void GetPathFileData_given_two_locations_when_paths_file_exists_in_second_location_then_return_parsed_path_file_object()
        {
            var p = typeof(vmBlock_Test).GetProperty("ContentRows");
            CreatePathDataLocations(new string[] { "c:/test", "c:/test2" });
            StubPathFile("c:/test", "vmBlock_Test", "not-here", false);
            StubPathFile("c:/test2", "vmBlock_Test", "path");

            var output = svc.Object.GetPathFileData(p.DeclaringType);

            Assert.AreEqual(JObject.Parse(jsonPaths), output);
        }

        [Test]
        public void GetPathFileData_given_path_file_found_then_throw_exception()
        {
            var p = typeof(vmBlock_Test).GetProperty("ContentRows");
            config.Setup(x => x.SchemaMetaLocations).Returns(new List<IDataLocation>());

            var ex = Assert.Throws<Exception>(() => svc.Object.GetPathFileData(p.DeclaringType));

            Assert.That(ex.Message == "Schema meta file not found for vmBlock_Test");
        }

        [Test]
        public void GetPossiblePathFileName_return_possible_paths_with_par_prefix_and_without_prefix()
        {
            var output = svc.Object.GetPossiblePathFileName("c:/test", "vmPage_Test");

            Assert.AreEqual("c:/test/Test.schema", output[0]);
            Assert.AreEqual("c:/test/parTest.schema", output[1]);
        }

        public void CreatePathDataLocations(string[] locations)
        {
            var schemaMetaLocations = locations.Select(x => new DataLocation() { Path = x }).Cast<IDataLocation>().ToList();

            config.Setup(x => x.SchemaMetaLocations).Returns(schemaMetaLocations);
        }

        public void StubPathFile(string rootPath, string declaringTypeName, string filePath, bool exists = true)
        {
            svc.Setup(x => x.GetPossiblePathFileName(rootPath, declaringTypeName)).Returns(new string[] { filePath });
            svc.Setup(x => x.FileExists(filePath)).Returns(exists);
            svc.Setup(x => x.FileRead(filePath)).Returns(jsonPaths);
        }

        public void StubConfigViewmodels(Type type)
        {
            var viewmodels = new List<Type>();
            viewmodels.Add(type);

            config.Setup(x => x.ViewModels).Returns(viewmodels);
        }

        public class vmBlock_Test
        {
            public string ContentRows { get; set; }
            public string ContentGrid { get; set; }
        }

    }
}
