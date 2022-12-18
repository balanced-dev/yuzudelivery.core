using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using System.Reflection;

namespace YuzuDelivery.Core.Test
{
    public class VmHelperExtensionsTests_GetComponent
    {
        public Type blockType;
        public Type subBlockType;
        public Type subListBlockType;
        public Type parentBlockType;
        public Type subExternalBlockType;

        public List<Type> ViewModels;

        public YuzuConfiguration config;

        [OneTimeSetUp]
        public void FixtureSetup()
        {
            YuzuConstants.Reset();
            YuzuConstants.Initialize(new YuzuConstantsConfig());
        }

        [SetUp]
        public void Setup()
        {
            blockType = Substitute.For<Type>();
            blockType.Name.Returns("vmBlock_Name");

            subBlockType = Substitute.For<Type>();
            subBlockType.Name.Returns("vmSub_Test");

            subListBlockType = Substitute.For<Type>();
            subListBlockType.IsGenericType.Returns(true);
            subListBlockType.GenericTypeArguments.Returns(new Type[] { subBlockType });

            parentBlockType = Substitute.For<Type>();
            parentBlockType.Name.Returns("vmSub_Parent");

            subExternalBlockType = Substitute.For<Type>();
            subExternalBlockType.Name.Returns("vmBlock_External");

            ViewModels = new List<Type>();
            ViewModels.Add(blockType);

            config = Substitute.ForPartsOf<YuzuConfiguration>(new List<IUpdateableConfig>());
            config.ViewModels.Returns(ViewModels);
        }

        [Test]
        public void GetComponent_when_type_is_component_the_just_return()
        {

            var output = blockType.GetComponent(config);

            Assert.That(output, Is.EqualTo(blockType));
        }

        [Test, Ignore("Moq doesn't support properties")]
        public void GetComponent_when_sub_is_child_object_then_return_parent()
        {
            SetupPropertyInfo(blockType, subBlockType);

            var output = subBlockType.GetComponent(config);

            Assert.That(output, Is.EqualTo(blockType));
        }

        [Test]
        public void GetComponent_when_sub_is_child_list_of_objects_then_return_parent()
        {
            SetupPropertyInfo(blockType, subListBlockType);

            var output = subBlockType.GetComponent(config);

            Assert.That(output, Is.EqualTo(blockType));
        }

        [Test]
        public void GetComponent_when_sub_is_grandchild_object_then_return_grandparent()
        {
            SetupPropertyInfo(blockType, parentBlockType);
            SetupPropertyInfo(parentBlockType, subBlockType);

            var output = subBlockType.GetComponent(config);

            Assert.That(output, Is.EqualTo(blockType));
        }

        [Test]
        public void GetComponent_when_sub_is_grandchild_list_of_objects_then_return_grandparent()
        {
            SetupPropertyInfo(blockType, parentBlockType);
            SetupPropertyInfo(parentBlockType, subListBlockType);

            var output = subBlockType.GetComponent(config);

            Assert.That(output, Is.EqualTo(blockType));
        }

        [Test]
        public void GetComponent_when_parent_is_block_then_return_null()
        {
            SetupPropertyInfo(blockType, subExternalBlockType);

            var output = subBlockType.GetComponent(config);

            Assert.IsNull(output);
        }

        public PropertyInfo SetupPropertyInfo(Type parent, Type child)
        {
            var propertyInfo = Substitute.For<PropertyInfo>();
            propertyInfo.PropertyType.Returns(child);

            parent.GetProperties().Returns(new PropertyInfo[] { propertyInfo });

            return propertyInfo;
        }

    }
}
