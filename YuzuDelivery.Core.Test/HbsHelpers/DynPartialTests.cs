using System.IO;
using HandlebarsDotNet;
using NUnit.Framework;
using YuzuDelivery.Core;

namespace YuzuDelivery.Core.Test.HbsHelpers
{
    
    public class DynPartial
    {
        private YuzuDelivery.Core.DynPartial helper;
        [SetUp]
        public void Setup()
        {
            helper = new YuzuDelivery.Core.DynPartial();
        }
        
        [Test]
        public void given_path_and_context_where_context_is_an_object()
        {
            var source = "{{{dynPartial 'partialName' foo}}}";
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

        
  
    }
}