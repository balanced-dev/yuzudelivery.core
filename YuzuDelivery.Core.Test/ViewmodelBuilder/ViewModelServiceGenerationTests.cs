using System;
using System.Collections.Generic;
using NUnit.Framework;
using Moq;
using YuzuDelivery.Core;
using YuzuDelivery.Core.ViewModelBuilder;

namespace YuzuDelivery.Core.ViewModelBuilder.Tests
{

    public class ViewModelBuilderServiceTests
    {
        public Mock<BuildViewModelsService> svcMock;
        public BuildViewModelsService svc;
        public ReferencesService references;

        [SetUp]
        public void Setup()
        {
            var rootDir = AppDomain.CurrentDomain.BaseDirectory.Replace("\\bin\\Debug\\net472", "").Replace("\\bin\\Release\\net472", "");
            var blockPath = string.Format("{0}\\ViewmodelBuilder\\Input", rootDir);

            var generate = new GenerateViewmodelService();
            var post = new List<IViewmodelPostProcessor>();

            var configMock = new Moq.Mock<IYuzuConfiguration>().SetupAllProperties();
            var config = configMock.Object;
            config.TemplateLocations = new List<ITemplateLocation>();
            config.TemplateLocations.Add(new TemplateLocation() { Name = "Pages", Schema = "some" });
            config.TemplateLocations.Add(new TemplateLocation() { Name = "Partials", Schema = blockPath });

            var importConfig = new Moq.Mock<IYuzuViewmodelsBuilderConfig>().SetupAllProperties().Object;
            importConfig.ExcludeViewmodelsAtGeneration = new List<string>();

            svcMock = new Moq.Mock<BuildViewModelsService>(MockBehavior.Strict, generate, post, config, importConfig) { CallBase = true };
            svcMock.Setup(x => x.WriteOutputFile(It.IsAny<string>(), It.IsAny<string>()));
            svc = svcMock.Object;
        }

        [Test]
        public void given_single_schema_with_simple_properties_then_class_name_is_prefixed_with_vm()
        {
            var file = svc.RunOneBlock(ViewModelType.block, "1_JustRootNode");

            Assert.IsTrue(file.Content.Contains("public partial class vmBlock_1_JustRootNode"));
        }

        [Test]
        public void given_single_schema_with_simple_properties_then_property_names_are_capitalised()
        {
            var file = svc.RunOneBlock(ViewModelType.block, "1_JustRootNode");

            Assert.IsTrue(file.Content.Contains("public string Discount"));
            Assert.IsTrue(file.Content.Contains("public string DiscountAmount"));
        }

        [Test]
        public void given_schema_with_an_external_ref_then_external_property_type_is_set_but_child_schema_is_not_generated()
        {
            var file = svc.RunOneBlock(ViewModelType.block,  "2_1_ExternalRef");

            Assert.IsTrue(file.Content.Contains("public vmBlock_2_2_ExternalRef Ref"));
            Assert.IsFalse(file.Content.Contains("public partial class vmBlock_2_2_ExternalRef"));
        }

        [Test]
        public void given_schema_with_an_internal_array_schema_then_internal_schame_created_using_singularized_property_name()
        {
            var file = svc.RunOneBlock(ViewModelType.block, "3_WithSubSchema_NamedArray");

            Assert.IsTrue(file.Content.Contains("public System.Collections.Generic.List<vmSub_3_WithSubSchema_NamedArrayItem> Items"));
            Assert.IsTrue(file.Content.Contains("public partial class vmSub_3_WithSubSchema_NamedArrayItem"));
        }

        [Test]
        public void given_schema_with_an_internal_named_schema_then_internal_schame_create_as_sub_object_with_name()
        {
            var file = svc.RunOneBlock(ViewModelType.block, "4_WithSubSchema_Named");

            Assert.IsTrue(file.Content.Contains("public vmSub_4_WithSubSchema_NamedItem Item"));
            Assert.IsTrue(file.Content.Contains("public partial class vmSub_4_WithSubSchema_NamedItem"));
        }

        [Test]
        public void given_schema_with_an_external_ref_with_child_schema_then_external_property_type_is_set_but_grandchild_schema_is_not_generated()
        {
            var file = svc.RunOneBlock(ViewModelType.block, "5_1_ExternalRefSub");

            Assert.IsFalse(file.Content.Contains("public partial class vmSub_5_2_ExternalRefSub_Named"));
        }

        [Test]
        public void given_schema_with_an_external_ref_using_module_then_external_property_types_are_set_but_module_schemas_are_not_generated()
        {
            var file = svc.RunOneBlock(ViewModelType.block, "6_1_ExternalRefModule");

            Assert.IsTrue(file.Content.Contains("public vmBlock_6_3_ExternalRefModule Module"));
            Assert.IsFalse(file.Content.Contains("public partial class vmBlock_6_3_ExternalRefModule2"));
        }

        [Test]
        public void given_schema_where_the_names_doesnt_match_the_throw_exceptiom()
        {
            var exception = string.Empty;

            try
            {
                svc.RunOneBlock(ViewModelType.block, "7_WrongRootName");
            }
            catch(Exception ex)
            {
                exception = ex.ToString();
            }

            Assert.IsTrue(exception.Contains("Filename and schema id must match, 7_WrongRootName doesn't equal 7_WrongRootName_Wrong and schema id must start with a /"));
        }


        [Test]
        public void given_schema_with_an_external_ref_as_an_array_then_array_is_created_with_the_correct_type()
        {
            var file = svc.RunOneBlock(ViewModelType.block, "8_1_ExternalRefSubArray");

            Assert.IsTrue(file.Content.Contains("System.Collections.Generic.List<vmBlock_8_2_ExternalRefSubArray> Ref"));
        }

        [Test]
        public void given_enum_in_child_schema_then_add_sucessfully()
        {
            var file = svc.RunOneBlock(ViewModelType.block, "9_EnumInSubObject");

            Assert.IsTrue(file.Content.Contains("public enum vmSub_9_EnumInSubObjectVmSub_9_EnumInSubObjectItemAllowed"));
        }

        [Test]
        public void given_any_of_array_then_add_as_object()
        {
            var file = svc.RunOneBlock(ViewModelType.block, "10_AnyOf_Array");

            Assert.IsTrue(file.Content.Contains("public System.Collections.Generic.List<object> Items { get; set; }"));
        }

        [Test]
        public void given_any_of_object_then_add_as_object()
        {
            var file = svc.RunOneBlock(ViewModelType.block, "11_AnyOf_Object");

            Assert.IsTrue(file.Content.Contains("public object Item { get; set; }"));
        }

        [Test]
        public void given_root_array_the_create_block_for_array()
        {
            var file = svc.RunOneBlock(ViewModelType.block, "12_Root_Array");

            Assert.IsTrue(file.Content.Contains("public partial class vmBlock_12_Root_Array"));
        }

        [Test]
        public void given_root_array_and_item_has_external_ref()
        {
            var file = svc.RunOneBlock(ViewModelType.block, "13_1_Root_Array_External_Ref");

            Assert.IsTrue(file.Content.Contains("public partial class vmBlock_13_2_ExternalRef"));
        }


        [Test]
        public void given_standalone_enum_then_create_enum()
        {
            var file = svc.RunOneBlock(ViewModelType.block, "14_DirectEnum");

            Assert.IsTrue(file.Content.Contains("public enum vmBlock_14_DirectEnum"));
        }

    }
}
