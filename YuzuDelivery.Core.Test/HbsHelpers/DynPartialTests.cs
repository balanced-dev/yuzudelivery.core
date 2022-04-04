using System.Collections.Generic;
using System.IO;
using HandlebarsDotNet;
using NUnit.Framework;
using YuzuDelivery.Core;

namespace YuzuDelivery.Core.Test.HbsHelpers
{
    public class vmBlock_PartialName
    {
       
        public vmBlock_PartialName()
        {
            Bar = "bar";
        }

        public string Bar { get; set; }
    }

    public class DynPartialTests
    {
        private YuzuDelivery.Core.DynPartial helper;

        [SetUp]
        public void Setup()
        {
            helper = new YuzuDelivery.Core.DynPartial();
        }

        [Test]
        public void given_empty_path_and_context()
        {
            var source = "{{{dynPartial '' foo}}}";
            var partialSource = "test {{bar}}";
            var template = Handlebars.Compile(source);

            using (var reader = new StringReader(partialSource))
            {
                var partialTemplate = Handlebars.Compile(reader);
                Handlebars.RegisterTemplate("parPartialName", partialTemplate);
            }

            var data = new { foo = new vmBlock_PartialName() };

            var output = template(data);
            Assert.AreEqual("test bar", output);
        }

        [Test]
        public void given_empty_path_context_and_parameter()
        {
            var source = "{{{dynPartial '' foo param='test'}}}";
            var partialSource = "test {{bar}} {{param}}";
            var template = Handlebars.Compile(source);

            using (var reader = new StringReader(partialSource))
            {
                var partialTemplate = Handlebars.Compile(reader);
                Handlebars.RegisterTemplate("parPartialName", partialTemplate);
            }

            var data = new { foo = new vmBlock_PartialName() };

            var output = template(data);
            Assert.AreEqual("test bar test", output);
        }

        [Test]
        public void given_empty_path_and_context_is_array()
        {
            var source = "{{{dynPartial '' foo}}}";
            var partialSource = "test {{#each this}}{{this.bar}}{{/each}}";
            var template = Handlebars.Compile(source);

            using (var reader = new StringReader(partialSource))
            {
                var partialTemplate = Handlebars.Compile(reader);
                Handlebars.RegisterTemplate("parPartialName", partialTemplate);
            }

            var data = new { foo = new[] { new vmBlock_PartialName() } };

            var output = template(data);
            Assert.AreEqual("test bar", output);
        }


        [Test]
        public void given_path_and_context_where_context_is_an_object()
        {
            var source = "{{{dynPartial 'partialName' foo}}}";
            var partialSource = "test {{bar}}";
            var template = Handlebars.Compile(source);

            using (var reader = new StringReader(partialSource))
            {
                var partialTemplate = Handlebars.Compile(reader);
                Handlebars.RegisterTemplate("partialName", partialTemplate);
            }

            var data = new
            {
                foo = new
                {
                    bar = "foo bar"
                }
            };

            var output = template(data);
            Assert.AreEqual("test foo bar", output);
        }

        [Test]
        [TestCase("foo bar")]
        [TestCase(12)]
        [TestCase(12.0)]
        [TestCase('f')]
        public void given_path_and_context_where_context_is_base_type(object input)
        {
            var source = "{{{dynPartial 'partialName' foo}}}";
            var partialSource = "test {{this}}";
            var template = Handlebars.Compile(source);

            using (var reader = new StringReader(partialSource))
            {
                var partialTemplate = Handlebars.Compile(reader);
                Handlebars.RegisterTemplate("partialName", partialTemplate);
            }

            var data = new
            {
                foo = input
            };

            var output = template(data);
            Assert.AreEqual($"test {input}", output);
        }
        
        [Test]
        public void given_empty_path_context_where_context_is_generic_type()
        {
            var source = "{{{dynPartial 'parPartialName' foo}}}";
            var partialSource = "test {{this.[0].bar}}";
            var template = Handlebars.Compile(source);

            using (var reader = new StringReader(partialSource))
            {
                var partialTemplate = Handlebars.Compile(reader);
                Handlebars.RegisterTemplate("parPartialName", partialTemplate);
            }

            var data = new { foo = new List<vmBlock_PartialName>() {new vmBlock_PartialName() } };

            var output = template(data);
            Assert.AreEqual("test bar", output);
        }


        [Test]
        public void given_path_context_and_parameter()
        {
            var source = "{{{dynPartial 'partialName' foo param='test'}}}";
            var partialSource = "test {{bar}} {{param}}";
            var template = Handlebars.Compile(source);

            using (var reader = new StringReader(partialSource))
            {
                var partialTemplate = Handlebars.Compile(reader);
                Handlebars.RegisterTemplate("partialName", partialTemplate);
            }

            var data = new
            {
                foo = new
                {
                    bar = "foo bar"
                }
            };

            var output = template(data);
            Assert.AreEqual("test foo bar test", output);
        }

        [Test]
        public void given_path_context_and_multiple_parameters()
        {
            var source = "{{{dynPartial 'partialName' foo param1='test' param2='test again'}}}";
            var partialSource = "test {{bar}} {{param1}} {{param2}}";
            var template = Handlebars.Compile(source);

            using (var reader = new StringReader(partialSource))
            {
                var partialTemplate = Handlebars.Compile(reader);
                Handlebars.RegisterTemplate("partialName", partialTemplate);
            }

            var data = new
            {
                foo = new
                {
                    bar = "foo bar"
                }
            };

            var output = template(data);
            Assert.AreEqual("test foo bar test test again", output);
        }

        [Test]
        public void given_path_no_context()
        {
            var source = "{{{dynPartial 'partialName'}}}";
            var partialSource = "test {{foo.bar}}";
            var template = Handlebars.Compile(source);

            using (var reader = new StringReader(partialSource))
            {
                var partialTemplate = Handlebars.Compile(reader);
                Handlebars.RegisterTemplate("partialName", partialTemplate);
            }

            var data = new
            {
                foo = new
                {
                    bar = "foo bar"
                }
            };

            var output = template(data);
            Assert.AreEqual("test foo bar", output);
        }

        [Test]
        public void given_path_no_context_with_parameter()
        {
            var source = "{{{dynPartial 'partialName' param='test'}}}";
            var partialSource = "test {{foo.bar}} {{param}}";
            var template = Handlebars.Compile(source);

            using (var reader = new StringReader(partialSource))
            {
                var partialTemplate = Handlebars.Compile(reader);
                Handlebars.RegisterTemplate("partialName", partialTemplate);
            }

            var data = new
            {
                foo = new
                {
                    bar = "foo bar"
                }
            };

            var output = template(data);
            Assert.AreEqual("test foo bar test", output);
        }
    }
}