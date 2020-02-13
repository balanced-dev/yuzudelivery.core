using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Rhino.Mocks;
using YuzuDelivery.Core;
using YuzuDelivery.Core.ViewModelBuilder;

namespace YuzuDelivery.Core.ViewModelBuilder.Tests
{

    public class ViewModelGetSchemaRefsTests
    {

        public ReferencesService svc;
        protected List<string> references;
        protected List<string> excludedTypes;

        [SetUp]
        public void Setup()
        {
            var config = MockRepository.GenerateStub<IYuzuConfiguration>();
            config.TemplateLocations = new List<ITemplateLocation>();
            config.TemplateLocations.Add(new TemplateLocation() { Name = "Pages", Schema = "some" });
            config.TemplateLocations.Add(new TemplateLocation() { Name = "Partials", Schema = "some" });

            svc = MockRepository.GeneratePartialMock<ReferencesService>(new object[] { config });
            references = new List<string>();
            excludedTypes = new List<string>();
        }

        [Test]
        public void given_a_file_with_name_references_single_space_after_colon_then_fix_to_file_references()
        {
            var file = "\"$ref\": \"/par2_2_ExternalRef\"";
            var output = svc.Fix(file);

            Assert.True(output.Contains("\"$ref\": \"./par2_2_ExternalRef.schema\""));
        }

        [Test]
        public void given_a_file_with_name_references_single_space_before_colon_then_fix_to_file_references()
        {
            var file = "\"$ref\" :\"/par2_2_ExternalRef\"";
            var output = svc.Fix(file);

            Assert.True(output.Contains("\"$ref\" :\"./par2_2_ExternalRef.schema\""));
        }

        [Test]
        public void given_a_file_with_name_references_with_double_space_then_fix_to_file_references()
        {
            var file = "\"$ref\" : \"/par2_2_ExternalRef\"";
            var output = svc.Fix(file);

            Assert.True(output.Contains("\"$ref\" : \"./par2_2_ExternalRef.schema\""));
        }

        [Test]
        public void given_a_file_with_name_references_no_spaces_then_fix_to_file_references()
        {
            var file = "\"$ref\":\"/par2_2_ExternalRef\"";
            var output = svc.Fix(file);

            Assert.True(output.Contains("\"$ref\":\"./par2_2_ExternalRef.schema\""));
        }

        [Test]
        public void given_a_file_with_multiple_name_references_then_fix_to_file_references()
        {
            var file = "\"$ref\": \"/par2_2_ExternalRef\"" + System.Environment.NewLine + "\"$ref\": \"/par1_2_ExternalRef\"";
            var output = svc.Fix(file);

            Assert.True(output.Contains("\"$ref\": \"./par2_2_ExternalRef.schema\""));
            Assert.True(output.Contains("\"$ref\": \"./par1_2_ExternalRef.schema\""));
        }

        [Test]
        public void given_a_file_with_multiple_name_references_of_the_same_ref_then_fix_to_file_reference_once_only()
        {
            var file = "\"$ref\": \"/par2_2_ExternalRef\"" + System.Environment.NewLine + "\"$ref\": \"/par2_2_ExternalRef\"";
            var output = svc.Fix(file);

            Assert.True(output.Contains("\"$ref\": \"./par2_2_ExternalRef.schema\""));
        }

        [Test]
        public void given_a_file_with_name_reference_and_partial_name_references_then_only_fix_file_references()
        {
            var file = "\"$ref\": \"/par_Ref\"" + System.Environment.NewLine + "\"$ref\": \"/par_Ref_Partial\"";
            var output = svc.Fix(file);

            Assert.True(output.Contains("\"$ref\": \"./par_Ref.schema\""));
            Assert.False(output.Contains("\"$ref\": \"./par_Ref.schema_Partial\""));
        }


    }
}
