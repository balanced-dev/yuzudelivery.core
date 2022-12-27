using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FluentAssertions;
using Fluid;
using Fluid.Values;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using YuzuDelivery.Core.Settings;
using YuzuDelivery.Core.ViewModelBuilder;

namespace YuzuDelivery.Core.Test.ViewmodelBuilder
{
    public class ViewModelBuilderServiceTests
    {
        BuildViewModelsService Sut { get; set; }

        IYuzuViewmodelsBuilderConfig BuilderConfig { get; set; }

        IOptions<CoreSettings> coreSettings;

        [SetUp]
        public void Setup()
        {
            Inflector.Inflector.SetDefaultCultureFunc = () => System.Threading.Thread.CurrentThread.CurrentUICulture;

            var blockPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ViewmodelBuilder", "Input");

            var generate = new GenerateViewmodelService();

            BuilderConfig = new YuzuViewmodelsBuilderConfig();

            var fileProvider = new EmbeddedFileProvider(Assembly.GetExecutingAssembly(), "YuzuDelivery.Core.ViewmodelBuilder.Input");

            coreSettings = Substitute.For<IOptions<CoreSettings>>();
            coreSettings.Value.Returns(Substitute.For<CoreSettings>());
            coreSettings.Value.SchemaFileProvider = fileProvider;

            Sut = Substitute.For<BuildViewModelsService>(generate, coreSettings, BuilderConfig);
            Sut.Configure().WriteOutputFile(Arg.Any<string>(), Arg.Any<string>());
        }

        [Test]
        public void given_single_schema_with_simple_properties_then_class_name_is_prefixed_with_vm()
        {
            var file = Sut.RunOneBlock(ViewModelType.block, "1_JustRootNode");

            Assert.IsTrue(file.Content.Contains("public partial class vmBlock_1_JustRootNode"));

            TestContext.Out.Write(file.Content);
        }

        [Test]
        public void given_single_schema_with_simple_properties_then_property_names_are_capitalised()
        {
            var file = Sut.RunOneBlock(ViewModelType.block, "1_JustRootNode");

            Assert.IsTrue(file.Content.Contains("public string Discount"));
            Assert.IsTrue(file.Content.Contains("public string DiscountAmount"));

            TestContext.Out.Write(file.Content);
        }

        [Test]
        public void given_schema_with_an_external_ref_then_external_property_type_is_set_but_child_schema_is_not_generated()
        {
            var file = Sut.RunOneBlock(ViewModelType.block,  "2_1_ExternalRef");

            Assert.IsTrue(file.Content.Contains("public vmBlock_2_2_ExternalRef Ref"));
            Assert.IsFalse(file.Content.Contains("public partial class vmBlock_2_2_ExternalRef"));

            TestContext.Out.Write(file.Content);
        }

        [Test]
        public void given_schema_with_an_internal_array_schema_then_internal_schame_created_using_singularized_property_name()
        {
            var file = Sut.RunOneBlock(ViewModelType.block, "3_WithSubSchema_NamedArray");

            Assert.IsTrue(file.Content.Contains("public System.Collections.Generic.List<vmSub_3_WithSubSchema_NamedArrayItem> Items"));
            Assert.IsTrue(file.Content.Contains("public partial class vmSub_3_WithSubSchema_NamedArrayItem"));

            TestContext.Out.Write(file.Content);
        }

        [Test]
        public void given_schema_with_an_internal_named_schema_then_internal_schame_create_as_sub_object_with_name()
        {
            var file = Sut.RunOneBlock(ViewModelType.block, "4_WithSubSchema_Named");

            Assert.IsTrue(file.Content.Contains("public vmSub_4_WithSubSchema_NamedItem Item"));
            Assert.IsTrue(file.Content.Contains("public partial class vmSub_4_WithSubSchema_NamedItem"));

            TestContext.Out.Write(file.Content);
        }

        [Test]
        public void given_schema_with_an_external_ref_with_child_schema_then_external_property_type_is_set_but_grandchild_schema_is_not_generated()
        {
            var file = Sut.RunOneBlock(ViewModelType.block, "5_1_ExternalRefSub");

            Assert.IsFalse(file.Content.Contains("public partial class vmSub_5_2_ExternalRefSub_Named"));

            TestContext.Out.Write(file.Content);
        }

        [Test]
        public void given_schema_with_an_external_ref_using_module_then_external_property_types_are_set_but_module_schemas_are_not_generated()
        {
            var file = Sut.RunOneBlock(ViewModelType.block, "6_1_ExternalRefModule");

            Assert.IsTrue(file.Content.Contains("public vmBlock_6_3_ExternalRefModule Module"));
            Assert.IsFalse(file.Content.Contains("public partial class vmBlock_6_3_ExternalRefModule2"));

            TestContext.Out.Write(file.Content);
        }

        [Test]
        public void given_schema_where_the_names_doesnt_match_the_throw_exceptiom()
        {
            var exception = string.Empty;

            try
            {
                Sut.RunOneBlock(ViewModelType.block, "7_WrongRootName");
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
            var file = Sut.RunOneBlock(ViewModelType.block, "8_1_ExternalRefSubArray");

            Assert.IsTrue(file.Content.Contains("System.Collections.Generic.List<vmBlock_8_2_ExternalRefSubArray> Ref"));

            TestContext.Out.Write(file.Content);
        }

        [Test]
        public void given_enum_in_child_schema_then_add_sucessfully()
        {
            var file = Sut.RunOneBlock(ViewModelType.block, "9_EnumInSubObject");

            Assert.IsTrue(file.Content.Contains("public enum vmSub_9_EnumInSubObjectVmSub_9_EnumInSubObjectItemAllowed"));

            TestContext.Out.Write(file.Content);
        }

        [Test]
        public void given_any_of_array_then_add_as_object()
        {
            var file = Sut.RunOneBlock(ViewModelType.block, "10_AnyOf_Array");

            Assert.IsTrue(file.Content.Contains("public System.Collections.Generic.List<object> Items { get; set; }"));

            TestContext.Out.Write(file.Content);
        }

        [Test]
        public void given_any_of_object_then_add_as_object()
        {
            var file = Sut.RunOneBlock(ViewModelType.block, "11_AnyOf_Object");

            Assert.IsTrue(file.Content.Contains("public object Item { get; set; }"));

            TestContext.Out.Write(file.Content);
        }

        [Test]
        public void given_root_array_the_create_block_for_array()
        {
            var file = Sut.RunOneBlock(ViewModelType.block, "12_Root_Array");

            Assert.IsTrue(file.Content.Contains("public partial class vmBlock_12_Root_Array"));

            TestContext.Out.Write(file.Content);
        }

        [Test]
        public void given_root_array_and_item_has_external_ref()
        {
            var file = Sut.RunOneBlock(ViewModelType.block, "13_1_Root_Array_External_Ref");

            Assert.IsTrue(file.Content.Contains("public partial class vmBlock_13_2_ExternalRef"));

            TestContext.Out.Write(file.Content);
        }


        [Test]
        public void given_standalone_enum_then_create_enum()
        {
            var file = Sut.RunOneBlock(ViewModelType.block, "14_DirectEnum");

            Assert.IsTrue(file.Content.Contains("public enum vmBlock_14_DirectEnum"));

            TestContext.Out.Write(file.Content);
        }

        [Test]
        public void RunOneBlock_SchemaWithAllOf_ProducesSubClass()
        {
            var file = Sut.RunOneBlock(ViewModelType.block, "15_yuzuDerivedClass");

            file.Content.Should().Contain("class vmBlock_15_yuzuDerivedClass : vmBlock_15_yuzuBaseClass");

            TestContext.Out.Write(file.Content);
        }

        [Test]
        public void RunOneBlock_WithAdditionalNamespacesInConfig_IncludesUsingDirectives()
        {
            BuilderConfig.AddNamespacesAtGeneration = new List<string>
            {
                "Yuzu.Test.Extra.One",
                "using Yuzu.Test.Extra.Two;"
            };

            var file = Sut.RunOneBlock(ViewModelType.block, "1_JustRootNode");

            file.Content.Should().MatchRegex(new Regex(@"^using Yuzu\.Test\.Extra\.One;", RegexOptions.Multiline));
            file.Content.Should().MatchRegex(new Regex(@"^using Yuzu\.Test\.Extra\.Two;", RegexOptions.Multiline)); // handles the legacy setup

            TestContext.Out.Write(file.Content);
        }

        [Test]
        public void RunOneBlock_ClassLevelAttributeTemplateInConfig_RendersAttribute()
        {
            BuilderConfig.ClassLevelAttributeTemplates = new Dictionary<string, string>
            {
                ["testing"] = @"[Foo(""{{ ClassName | strip_vm_prefix }}"")]"
            };

            var file = Sut.RunOneBlock(ViewModelType.block, "1_JustRootNode");

            file.Content.Should().MatchRegex(new Regex(@"\[Foo\(""1_JustRootNode""\)\]\s+public partial class", RegexOptions.Multiline));

            TestContext.Out.Write(file.Content);
        }

        [Test]
        public void RunOneBlock_CustomFilterRegisteredInConfig_CustomFilterIsUsable()
        {
            BuilderConfig.ClassLevelAttributeTemplates = new Dictionary<string, string>
            {
                ["testing"] = @"[Foo(""{{ ClassName | strip_vm_prefix | bar }}"")]"
            };

            BuilderConfig.CustomFilters.Add(KeyValuePair.Create<string,FilterDelegate>("bar", (fv, fa, tc)
                => new ValueTask<FluidValue>(new StringValue("bar", encode: false))));

            var file = Sut.RunOneBlock(ViewModelType.block, "1_JustRootNode");

            file.Content.Should().Contain(@"[Foo(""bar"")]");

            TestContext.Out.Write(file.Content);
        }

        [Test]
        public void RunOneBlock_WithConfiguredTemplateAssemblies_RespectsDownstreamTemplates()
        {
            BuilderConfig.TemplateAssemblies.Add(GetType().Assembly);

            var file = Sut.RunOneBlock(ViewModelType.block, "1_JustRootNode");

            file.Content.Should().Be("YuzuDelivery.Core.Tests.File");

            TestContext.Out.Write(file.Content);
        }
    }
}
