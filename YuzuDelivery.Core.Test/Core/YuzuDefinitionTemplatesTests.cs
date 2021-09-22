using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Moq;
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
        private Mock<IMapper> mapper;

        private Mock<YuzuDefinitionTemplates> svc;
        private RenderSettings settings;
        private Mock<IYuzuTypeFactoryRunner> typeFactoryRunner;

        private ExampleModel exampleModel;
        private IDictionary<string, object> inputMappingItems;
        private IDictionary<string, object> usedMappingItems;

        private vmPage_ExampleViewModel exampleViewModel;

        private object dataObject;
        private string html;
        private Mock<IYuzuConfiguration> config;

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
            mapper = new Moq.Mock<IMapper>();
            config = new Moq.Mock<IYuzuConfiguration>().SetupAllProperties();
            typeFactoryRunner = new Moq.Mock<IYuzuTypeFactoryRunner>();
            mapperAddItems = new List<IMapperAddItem>();

            svc = new Moq.Mock<YuzuDefinitionTemplates>(MockBehavior.Loose, mapper.Object, config.Object, mapperAddItems.ToArray(), typeFactoryRunner.Object) { CallBase = true };
            settings = new RenderSettings();

            config.Object.GetRenderedHtmlCache = null;
            config.Object.SetRenderedHtmlCache = null;

            templates = new Dictionary<string, Func<object, string>>();

            templateName = "template";
            templateRenderer = (object data) => { return html; };
            config.Object.GetTemplatesCache = () => { return templates; };

            exampleModel = new ExampleModel() { Text = "text" };
            exampleViewModel = new vmPage_ExampleViewModel();
            inputMappingItems = new Dictionary<string, object>();

            Func<object, IDictionary<string, object>, vmPage_ExampleViewModel> doFunction = (object source, IDictionary<string, object> items) => {
                usedMappingItems = items; return exampleViewModel;
            };

            mapper.Setup(x => x.Map<vmPage_ExampleViewModel>(It.IsAny<object>(), It.IsAny<IDictionary<string, object>>())).Returns(doFunction);

            Mapper.Reset();
        }

        #region render from mappings

        [Test]
        public void given_automapped_model_then_model_maps_to_viewModel()
        {
            StubRenderMethod();

            mapper.Setup(x => x.Map<vmPage_ExampleViewModel>(It.IsAny<object>(), It.IsAny<IDictionary<string, object>>())).Returns(exampleViewModel);

            svc.Object.Render<vmPage_ExampleViewModel>(exampleModel, false, settings);

            Assert.AreEqual(settings.Data(), exampleViewModel);
        }

        [Test]
        public void given_automapped_model_and_mapping_items_then_model_maps_to_viewModel()
        {
            StubRenderMethod();

            inputMappingItems.Add("test", "text");

            svc.Object.Render<vmPage_ExampleViewModel>(exampleModel, false, settings, null, inputMappingItems);

            settings.Data();

            Assert.AreEqual(inputMappingItems["test"], usedMappingItems["test"]);
        }

        [Test]
        public void given_automapped_model_and_mapped_actions_then_apply()
        { 
            var mappedItem = new Moq.Mock<IMapperAddItem>();

            mapperAddItems.Add(mappedItem.Object);

            svc = new Moq.Mock<YuzuDefinitionTemplates>(MockBehavior.Loose, mapper.Object, config.Object, mapperAddItems.ToArray(), typeFactoryRunner.Object) { CallBase = true };

            StubRenderMethod();

            svc.Object.Render<vmPage_ExampleViewModel>(exampleModel, false, settings, null, inputMappingItems);

            mappedItem.Verify(x => x.Add(inputMappingItems));
        }

        [Test]
        public void given_typedfactory_model_then_use_it()
        {
            StubRenderMethod();

            typeFactoryRunner.Setup(x => x.Run<vmPage_ExampleViewModel>(null)).Returns(exampleViewModel);

            svc.Object.Render<vmPage_ExampleViewModel>(exampleModel, false, settings, null, inputMappingItems);

            Assert.AreEqual(settings.Data(), exampleViewModel);
            typeFactoryRunner.Verify(x => x.Run<vmPage_ExampleViewModel>(inputMappingItems));
        }

        public void StubRenderMethod()
        {
            svc.Setup(x => x.Render(settings)).Returns((string) null);
        }

        #endregion

        #region render process

        [Test]
        public void given_render_settings_null_then_throw_exception()
        {
            Assert.Throws<ArgumentNullException>(() => svc.Object.Render(null));
        }

        [Test]
        public void given_no_cache_run_all_render_processes()
        {
            RemoveAllMethodsFromRender();

            svc.Object.Render(settings);

            svc.Verify(x => x.CreateData(settings));
            svc.Verify(x => x.RenderTemplate(settings, dataObject));
            svc.Verify(x => x.AddCurrentJsonToTemplate(settings, dataObject, html));
        }

        [Test]
        public void given_cache_name_and_cache_processes_not_set_then_run_process()
        {
            RemoveAllMethodsFromRender();

            settings.CacheName = "cacheName";

            var output = svc.Object.Render(settings);

            svc.Verify(x => x.RenderTemplate(settings, dataObject));
        }

        [Test]
        public void given_cache_name_and_cache_exists_then_used_cached_version_and_dont_run_render()
        {
            config.Object.GetRenderedHtmlCache = (IRenderSettings settings) => { return html; };

            html = "html";
            settings.CacheName = "cacheName";

            var output = svc.Object.Render(settings);

            Assert.AreEqual(html, output);
        }

        [Test]
        public void given_cache_name_and_no_cache_then_add_rendered_to_cache()
        {
            var output = string.Empty;
            html = "html";

            config.Object.GetRenderedHtmlCache = (IRenderSettings settings) => { return null; };
            config.Object.SetRenderedHtmlCache = (IRenderSettings settings, string html) => { output = html; };

            RemoveAllMethodsFromRender();

            settings.CacheName = "cacheName";

            svc.Object.Render(settings);

            Assert.AreEqual(html, output);
        }

        public void RemoveAllMethodsFromRender()
        {
            svc.Setup(x => x.CreateData(settings)).Returns(dataObject);
            svc.Setup(x => x.RenderTemplate(settings, dataObject)).Returns(html);
            svc.Setup(x => x.AddCurrentJsonToTemplate(settings, dataObject, html)).Returns(html);
        }

        #endregion

        #region data

        [Test]
        public void given_render_data_then_settings_run_is_run()
        {
            var data = "test";

            settings.Data = () => { return data; };

            var output = svc.Object.CreateData(settings);

            Assert.AreEqual(data, output);
        }

        [Test]
        public void given_no_render_data_then_return_empty_object()
        {
            var output = svc.Object.CreateData(settings);

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

            var output = svc.Object.RenderTemplate(settings, dataObject);

            Assert.AreEqual(html, output);
        }

        [Test]
        public void given_data_is_null_then_templates_render()
        {
            html = "html";

            AddTemplatesToTemplates();

            var output = svc.Object.RenderTemplate(settings, null);

            Assert.AreEqual(html, output);
        }

        [Test]
        public void given_templates_when_template_not_present_then_throw_exception()
        {
            Assert.Throws<ArgumentNullException>(() => svc.Object.RenderTemplate(settings, dataObject));
        }

        [Test]
        public void given_no_templates_then_reload_templates()
        {
            var output = false;

            AddTemplatesToTemplates();

            config.Object.GetTemplatesCache = () => { return null; };
            config.Object.SetTemplatesCache = () => { output = true; return templates; };

            svc.Object.RenderTemplate(settings, dataObject);

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
            var output = svc.Object.GetSuspectTemplateName(typeof(vmPage_ExampleViewModel));

            Assert.AreEqual("exampleViewModel", output);
        }

        [Test]
        public void give_page_viewmodel_the_return_partial_template_name()
        {
            var output = svc.Object.GetSuspectTemplateName(typeof(vmBlock_ExampleViewModelSub));

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

            var output = svc.Object.AddCurrentJsonToTemplate(settings, dataObject, html);

            Assert.IsTrue(output.StartsWith(html));
            Assert.IsTrue(output.Contains("name"));
            Assert.IsTrue(output.Contains("test"));
        }

        [Test]
        public void given_settings_adds_json_and_data_is_null_then_html_unchanged()
        {
            html = "<h1>header</h1>";
            settings.ShowJson = true;

            var output = svc.Object.AddCurrentJsonToTemplate(settings, null, html);

            Assert.AreEqual(html, output);
        }

        [Test]
        public void given_settings_adds_json_and_data_is_empty_string_then_html_unchanged()
        {
            html = "<h1>header</h1>";
            settings.ShowJson = true;

            var output = svc.Object.AddCurrentJsonToTemplate(settings, string.Empty, html);

            Assert.AreEqual(html, output);
        }

        [Test]
        public void given_setting_doesnt_add_json_then_html_unchanged()
        {
            var output = svc.Object.AddCurrentJsonToTemplate(settings, dataObject, html);

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
                OptionItem = context.Options.Items["test"].ToString()
            };
        }
    }
}
