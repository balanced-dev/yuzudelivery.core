using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Fluid;
using Fluid.Ast;
using Microsoft.Extensions.Options;
using NJsonSchema.CodeGeneration;
using NJsonSchema.CodeGeneration.CSharp;
using Parlot.Fluent;
using YuzuDelivery.Core.Settings;
using YuzuDelivery.Core.ViewModelBuilder;

namespace YuzuDelivery.Core.ViewmodelBuilder.NJsonSchema.CodeGeneration;

public class YuzuTemplateFactory : ITemplateFactory
{
    private readonly ViewModelGenerationSettings _config;
    private readonly TemplateOptions _templateOptions;
    private readonly Assembly[] _templateAssemblies;
    private readonly FluidParser _fluidParser;

    public YuzuTemplateFactory(ViewModelGenerationSettings config)
    {
        _config = config;
        _templateOptions = new TemplateOptions();

        _templateOptions = new TemplateOptions
        {
            MemberAccessStrategy = new UnsafeMemberAccessStrategy(),
            CultureInfo = CultureInfo.InvariantCulture,
            Greedy = false
        };

        _templateOptions.Filters.AddFilter("csharpdocs", LiquidFilters.CSharpDocs);
        _templateOptions.Filters.AddFilter("lowercamelcase", LiquidFilters.CamelCase);
        _templateOptions.Filters.AddFilter("uppercamelcase", LiquidFilters.PascalCase);
        _templateOptions.Filters.AddFilter("literal", LiquidFilters.Literal);
        _templateOptions.Filters.AddFilter("strip_vm_prefix", LiquidFilters.StripVmTypePrefix);
        _templateOptions.Filters.AddFilter("strip_using_directive", LiquidFilters.StripUsingDirective);

        foreach (var filter in config.CustomFilters)
        {
            _templateOptions.Filters.AddFilter(filter.Key, filter.Value);
        }

        _templateAssemblies = config.TemplateAssemblies.Concat(new[]
        {
            Assembly.GetExecutingAssembly(), // 1) Our custom templates
            typeof(CSharpGenerator).Assembly // 2) Fallback to upstream templates
        }).ToArray();

        _fluidParser = new YuzuFluidParser(this);
    }

    public ITemplate CreateTemplate(string language, string templateName, object model)
    {
        var fluidTemplate = GetFluidTemplate(language, templateName);

        return new YuzuCodeTemplate(fluidTemplate, language, templateName, _templateOptions, _config, model);
    }

    private bool TryGetSettingsTemplate(string templateName, out string template)
    {
        if (_config.ClassLevelAttributeTemplates.ContainsKey(templateName))
        {
            template = _config.ClassLevelAttributeTemplates[templateName];
            return true;
        }

        template = null;
        return false;
    }


    private string GetTemplateString(string language, string templateName)
    {
        if (TryGetSettingsTemplate(templateName, out var settingTemplate))
        {
            return settingTemplate;
        }

        foreach (var templateAssembly in _templateAssemblies)
        {
            var resourceName = templateAssembly
                               .GetManifestResourceNames()
                               .FirstOrDefault(x => x.EndsWith($"{templateName}.liquid"));

            if (resourceName == null)
            {
                continue;
            }

            var resource = templateAssembly.GetManifestResourceStream(resourceName);

            if (resource == null)
            {
                continue;
            }

            using var reader = new StreamReader(resource);
            return reader.ReadToEnd();
        }

        throw new InvalidOperationException($"Could not load template '{templateName}' for language '{language}'");
    }

    private IFluidTemplate GetFluidTemplate(string language, string templateName)
    {
        var rawTemplate = GetTemplateString(language, templateName);
        return _fluidParser.Parse(rawTemplate);
    }

    private class YuzuCodeTemplate : ITemplate
    {
        private readonly IFluidTemplate _template;
        private readonly string _language;
        private readonly string _templateName;
        private readonly TemplateOptions _templateOptions;
        private readonly ViewModelGenerationSettings _yuzuConfig;
        private readonly object _model;

        public YuzuCodeTemplate(IFluidTemplate template, string language, string templateName,
            TemplateOptions templateOptions, ViewModelGenerationSettings yuzuConfig, object model)
        {
            _template = template;
            _language = language;
            _templateName = templateName;
            _templateOptions = templateOptions;
            _yuzuConfig = yuzuConfig;
            _model = model;
        }

        public string Render()
        {
            TemplateContext templateContext = null;
            var childScope = false;

            try
            {
                if (_model is TemplateContext parentContext)
                {
                    templateContext = parentContext;
                    templateContext.EnterChildScope();
                    childScope = true;
                }
                else
                {
                    templateContext = new TemplateContext(_model, _templateOptions)
                    {
                        AmbientValues =
                        {
                            [YuzuFluidParser.LanguageKey] = _language,
                            [YuzuFluidParser.TemplateKey] = _templateName,
                        }
                    };

                    var version = typeof(YuzuTemplateFactory).Assembly
                                          .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
                                          .InformationalVersion
                                  ?? string.Empty;

                    templateContext.SetValue("Yuzu", _yuzuConfig);
                    templateContext.SetValue("ToolchainVersion", version);
                }

                return _template.Render(templateContext);
            }
            finally
            {
                if (childScope)
                {
                    templateContext.ReleaseScope();
                }
            }
        }
    }

    private class YuzuFluidParser : FluidParser
    {
        private readonly ITemplateFactory _templateFactory;
        internal const string LanguageKey = "__language";
        internal const string TemplateKey = "__template";

        public YuzuFluidParser(ITemplateFactory templateFactory)
        {
            _templateFactory = templateFactory;
            RegisterParserTag("template", Parsers.OneOrMany(Primary), RenderTemplate);
        }

        private string GetTemplateName(Expression expression, TemplateContext context)
        {
            string templateName = null;

            // ReSharper disable once ConvertIfStatementToSwitchStatement
            if (expression is LiteralExpression le)
            {
                templateName = le.Value.ToStringValue();
            }
            else if (expression is MemberExpression me)
            {
                var items = me.Segments
                              .OfType<IdentifierSegment>()
                              .Select(x => x.Identifier);

                templateName = string.Join('.', items);
            }

            var fromContext = context.GetValue(templateName);
            if (!string.IsNullOrEmpty(fromContext.ToStringValue()))
            {
                templateName = fromContext.ToStringValue();
            }

            if (string.IsNullOrEmpty(templateName))
            {
                return context.AmbientValues[TemplateKey] + "!";
            }

            return templateName;
        }

        private ValueTask<Completion> RenderTemplate(
            List<Expression> arguments,
            TextWriter writer,
            TextEncoder encoder,
            TemplateContext context)
        {
            var templateName = GetTemplateName(arguments[0], context);

            var language = (string) context.AmbientValues[LanguageKey];

            var template = _templateFactory.CreateTemplate(language, templateName, context);
            var output = template.Render();

            writer.Write(output);

            return new ValueTask<Completion>(Completion.Normal);
        }
    }
}
