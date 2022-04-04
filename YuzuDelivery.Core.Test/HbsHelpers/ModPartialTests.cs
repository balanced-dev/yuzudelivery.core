using System.Collections.Generic;
using System.IO;
using HandlebarsDotNet;
using NUnit.Framework;
using YuzuDelivery.Core;

namespace YuzuDelivery.Core.Test.HbsHelpers
{
    
    public class ModPartialTests
    {
        private YuzuDelivery.Core.ModPartial helper;
        [SetUp]
        public void Setup()
        {
            helper = new YuzuDelivery.Core.ModPartial();
        }
        
                [Test]
        public void given_empty_path_and_context()
        {
            var source = "{{{modPartial '' foo ''}}}";
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
            var source = "{{{modPartial '' foo '' param='test'}}}";
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
            var source = "{{{modPartial '' foo ''}}}";
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
        public void given_path_and_context_where_context_is_an_object_and_modifiers_is_empty()
        {
            var source = "{{{modPartial 'partialName' foo ''}}}";
            var partialSource = "test {{bar}}";
            var template = Handlebars.Compile(source);
            
            using(var reader = new StringReader(partialSource))
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
        public void given_path_context_and_parameter()
        {
            var source = "{{{modPartial 'partialName' foo param='test'}}}";
            var partialSource = "test {{bar}} {{param}}";
            var template = Handlebars.Compile(source);
            
            using(var reader = new StringReader(partialSource))
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
            var source = "{{{modPartial 'partialName' foo param1='test' param2='test again'}}}";
            var partialSource = "test {{bar}} {{param1}} {{param2}}";
            var template = Handlebars.Compile(source);
            
            using(var reader = new StringReader(partialSource))
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

    }
}