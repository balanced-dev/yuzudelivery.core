using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using AutoMapper;
using Newtonsoft.Json;
#if NETCOREAPP
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
#else
using System.Web.Mvc;
#endif

namespace YuzuDelivery.Core.Test
{
    [TestFixture]
    public class YuzuDefinitionTemplatesTests
    {
        private IMapper mapper;

        private YuzuDefinitionTemplates svc;
        private RenderSettings settings;
        private IYuzuTypeFactoryRunner typeFactoryRunner;

        private ExampleModel exampleModel;
        private IDictionary<string, object> inputMappingItems;
        private IDictionary<string, object> usedMappingItems;

        private vmPage_ExampleViewModel exampleViewModel;

        private object dataObject;
        private string html;
        private IYuzuConfiguration config;

        List<IMapperAddItem> mapperAddItems;

        private Dictionary<string, Func<object, string>> templates;
        private string templateName;
        private Func<object, string> templateRenderer;

        [OneTimeSetUp]
        public void SetupFixture()
        {
            YuzuConstants.Reset();
            YuzuConstants.Initialize(new YuzuConstantsConfig());
        }

        [SetUp]
        public void Setup()
        {
            mapper = Substitute.For<IMapper>();
            config = Substitute.For<IYuzuConfiguration>();
            typeFactoryRunner = Substitute.For<IYuzuTypeFactoryRunner>();
            mapperAddItems = new List<IMapperAddItem>();

            svc = Substitute.ForPartsOf<YuzuDefinitionTemplates>(mapper, config, mapperAddItems.ToArray(), typeFactoryRunner);
            settings = new RenderSettings();

            config.GetRenderedHtmlCache = null;
            config.SetRenderedHtmlCache = null;

            templates = new Dictionary<string, Func<object, string>>();

            templateName = "template";
            templateRenderer = (object data) => { return html; };
            config.GetTemplatesCache = () => { return templates; };

            exampleModel = new ExampleModel() { Text = "text" };
            exampleViewModel = new vmPage_ExampleViewModel();
            inputMappingItems = new Dictionary<string, object>();

            Func<object, IDictionary<string, object>, vmPage_ExampleViewModel> doFunction = (object source, IDictionary<string, object> items) => {
                usedMappingItems = items; return exampleViewModel;
            };

            mapper.WhenForAnyArgs(x => x.Map<vmPage_ExampleViewModel>(null, null)).Do(x => doFunction(x.Arg<object>(), x.Arg<IDictionary<string, object>>()));

        }

        #region render from mappings

        [Test]
        public void given_automapped_model_then_model_maps_to_viewModel()
        {
            StubRenderMethod();

            mapper.Map<vmPage_ExampleViewModel>(Arg.Any<object>(), Arg.Any<IDictionary<string, object>>()).Returns(exampleViewModel);

            svc.Render<vmPage_ExampleViewModel>(exampleModel, false, settings);

            Assert.AreEqual(settings.Data(), exampleViewModel);
        }

        [Test]
        public void given_automapped_model_and_mapping_items_then_model_maps_to_viewModel()
        {
            StubRenderMethod();

            inputMappingItems.Add("test", "text");

            svc.Render<vmPage_ExampleViewModel>(exampleModel, false, settings, null, inputMappingItems);

            settings.Data();

            Assert.AreEqual(inputMappingItems["test"], usedMappingItems["test"]);
        }

        [Test]
        public void given_automapped_model_and_mapped_actions_then_apply()
        {
            var mappedItem = Substitute.For<IMapperAddItem>();

            mapperAddItems.Add(mappedItem);

            svc = Substitute.For<YuzuDefinitionTemplates>(mapper, config, mapperAddItems.ToArray(), typeFactoryRunner);

            StubRenderMethod();

            svc.Render<vmPage_ExampleViewModel>(exampleModel, false, settings, null, inputMappingItems);

            mappedItem.Received().Add(inputMappingItems);
        }

        [Test]
        public void given_typedfactory_model_then_use_it()
        {
            StubRenderMethod();

            typeFactoryRunner.Run<vmPage_ExampleViewModel>(inputMappingItems).Returns(exampleViewModel);

            svc.Render<vmPage_ExampleViewModel>(exampleModel, false, settings, null, inputMappingItems);

            Assert.AreEqual(settings.Data(), exampleViewModel);
            typeFactoryRunner.Received().Run<vmPage_ExampleViewModel>(inputMappingItems);
        }

        public void StubRenderMethod()
        {
            svc.Configure().Render(settings).Returns((string) null);
        }

        #endregion

        #region render process

        [Test]
        public void given_render_settings_null_then_throw_exception()
        {
            Assert.Throws<ArgumentNullException>(() => svc.Render(null));
        }

        [Test]
        public void given_no_cache_run_all_render_processes()
        {
            RemoveAllMethodsFromRender();

            svc.Render(settings);

            svc.Received().CreateData(settings);
            svc.Received().RenderTemplate(settings, dataObject);
            svc.Received().AddCurrentJsonToTemplate(settings, dataObject, html);
        }

        [Test]
        public void given_cache_name_and_cache_processes_not_set_then_run_process()
        {
            RemoveAllMethodsFromRender();

            settings.CacheName = "cacheName";

            var output = svc.Render(settings);

            svc.Received().RenderTemplate(settings, dataObject);
        }

        [Test]
        public void given_cache_name_and_cache_exists_then_used_cached_version_and_dont_run_render()
        {
            config.GetRenderedHtmlCache = (IRenderSettings settings) => { return html; };

            html = "html";
            settings.CacheName = "cacheName";

            var output = svc.Render(settings);

            Assert.AreEqual(html, output);
        }

        [Test]
        public void given_cache_name_and_no_cache_then_add_rendered_to_cache()
        {
            var output = string.Empty;
            html = "html";

            config.GetRenderedHtmlCache = (IRenderSettings settings) => { return null; };
            config.SetRenderedHtmlCache = (IRenderSettings settings, string html) => { output = html; };

            RemoveAllMethodsFromRender();

            settings.CacheName = "cacheName";

            svc.Render(settings);

            Assert.AreEqual(html, output);
        }

        public void RemoveAllMethodsFromRender()
        {
            svc.Configure().CreateData(settings).Returns(dataObject);
            svc.Configure().RenderTemplate(settings, dataObject).Returns(html);
            svc.Configure().AddCurrentJsonToTemplate(settings, dataObject, html).Returns(html);
        }

        #endregion

        #region data

        [Test]
        public void given_render_data_then_settings_run_is_run()
        {
            var data = "test";

            settings.Data = () => { return data; };

            var output = svc.CreateData(settings);

            Assert.AreEqual(data, output);
        }

        [Test]
        public void given_no_render_data_then_return_empty_object()
        {
            var output = svc.CreateData(settings);

            Assert.AreEqual(JsonConvert.SerializeObject(output), "{}");
        }

        #endregion

        #region rendertemplate

        [Test]
        public void given_templates_when_template_requested_then_render_and_return_html()
        {
            dataObject = new { };
            html = "html";

            AddTemplatesToTemplates();

            var output = svc.RenderTemplate(settings, dataObject);

            Assert.AreEqual(html, output);
        }

        [Test]
        public void given_data_is_null_then_templates_render()
        {
            html = "html";

            AddTemplatesToTemplates();

            var output = svc.RenderTemplate(settings, null);

            Assert.AreEqual(html, output);
        }

        [Test]
        public void given_templates_when_template_not_present_then_throw_exception()
        {
            Assert.Throws<ArgumentNullException>(() => svc.RenderTemplate(settings, dataObject));
        }

        [Test]
        public void given_no_templates_then_reload_templates()
        {
            var output = false;

            AddTemplatesToTemplates();

            config.GetTemplatesCache = () => { return null; };
            config.SetTemplatesCache = () => { output = true; return templates; };

            svc.RenderTemplate(settings, dataObject);

            Assert.IsTrue(output);
        }

        public void AddTemplatesToTemplates()
        {
            templates.Add(templateName, templateRenderer);
            settings.Template = templateName;
        }


        #endregion

        #region automate template name

        [Test]
        public void give_page_viewmodel_the_return_page_template_name()
        {
            var output = svc.GetSuspectTemplateNameFromVm(typeof(vmPage_ExampleViewModel));

            Assert.AreEqual("exampleViewModel", output);
        }

        [Test]
        public void give_page_viewmodel_the_return_partial_template_name()
        {
            var output = svc.GetSuspectTemplateNameFromVm(typeof(vmBlock_ExampleViewModelSub));

            Assert.AreEqual("parExampleViewModelSub", output);
        }

        #endregion

        #region add_json

        [Test]
        public void given_settings_adds_json_then_add_json_output_to_html()
        {
            html = "<h1>header</h1>";
            dataObject = new { name = "test" };
            settings.ShowJson = true;

            var output = svc.AddCurrentJsonToTemplate(settings, dataObject, html);

            Assert.IsTrue(output.StartsWith(html));
            Assert.IsTrue(output.Contains("name"));
            Assert.IsTrue(output.Contains("test"));
        }

        [Test]
        public void given_settings_adds_json_and_data_is_null_then_html_unchanged()
        {
            html = "<h1>header</h1>";
            settings.ShowJson = true;

            var output = svc.AddCurrentJsonToTemplate(settings, null, html);

            Assert.AreEqual(html, output);
        }

        [Test]
        public void given_settings_adds_json_and_data_is_empty_string_then_html_unchanged()
        {
            html = "<h1>header</h1>";
            settings.ShowJson = true;

            var output = svc.AddCurrentJsonToTemplate(settings, string.Empty, html);

            Assert.AreEqual(html, output);
        }

        [Test]
        public void given_setting_doesnt_add_json_then_html_unchanged()
        {
            var output = svc.AddCurrentJsonToTemplate(settings, dataObject, html);

            Assert.AreEqual(html, output);
        }

        #endregion

    }

    public class ExampleModel
    {
        public string Text { get; set; }
    }

    public class vmPage_ExampleViewModel
    {
        public string Text { get; set; }
        public vmBlock_ExampleViewModelSub Sub { get; set; }
    }

    public class vmBlock_ExampleViewModelSub
    {
        public string OptionItem { get; set; }
    }


    public class ExampleConverter : IValueConverter<string, vmBlock_ExampleViewModelSub>
    {

        public vmBlock_ExampleViewModelSub Convert(string sourceMember, ResolutionContext context)
        {
            return new vmBlock_ExampleViewModelSub()
            {
                OptionItem = context.Items["test"].ToString()
            };
        }
    }
}
