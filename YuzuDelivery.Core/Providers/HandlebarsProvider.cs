using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using HandlebarsDotNet;

namespace YuzuDelivery.Umbraco.Blocks
{
    public interface IHandlebarsProvider
    {
        Func<object, string> Compile(string template);
        Action<TextWriter, object> Compile(TextReader template);
        void RegisterTemplate(string templateName, Action<TextWriter, object> template);
    }

    public class HandlebarsProvider : IHandlebarsProvider
    {
        public Func<object, string> Compile(string template)
        {
            return Handlebars.Compile(template);
        }

        public Action<TextWriter, object> Compile(TextReader template)
        {
            return Handlebars.Compile(template);
        }

        public void RegisterTemplate(string templateName, Action<TextWriter, object> template)
        {
            Handlebars.RegisterTemplate(templateName, template);
        }
    }
}
