using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Moq;
using System.Reflection;

namespace YuzuDelivery.Core.Test
{
    public class VmHelperExtensionsTests_GetComponent
    {
        public Mock<Type> blockType;
        public Mock<Type> subBlockType;
        public Mock<Type> subListBlockType;
        public Mock<Type> parentBlockType;
        public Mock<Type> subExternalBlockType;

        public List<Type> ViewModels;

        public Mock<YuzuConfiguration> config;

        [OneTimeSetUp]
        public void FixtureSetup()
        {
            YuzuConstants.Reset();
            YuzuConstants.Initialize(new YuzuConstantsConfig());
        }

        [SetUp]
        public void Setup()
        {
            blockType = new Moq.Mock<Type>();
            blockType.Setup(x => x.Name).Returns("vmBlock_Name");

            subBlockType = new Moq.Mock<Type>();
            subBlockType.Setup(x => x.Name).Returns("vmSub_Test");

            /*subListBlockType = MockRepository.GenerateStub<Type>();
            subListBlockType.Stub(x => x.IsGenericType).Return(true);
            subListBlockType.Stub(x => x.GenericTypeArguments).Return(new Type[] { subBlockType });

            parentBlockType = MockRepository.GenerateStub<Type>();
            parentBlockType.Stub(x => x.Name).Return("vmSub_Parent");

            subExternalBlockType = MockRepository.GenerateStub<Type>();
            subExternalBlockType.Stub(x => x.Name).Return("vmBlock_External");*/

            ViewModels = new List<Type>();
            ViewModels.Add(blockType.Object);

            config = new Moq.Mock<YuzuConfiguration>(MockBehavior.Loose, new List<IUpdateableConfig>()) { CallBase = true };
            config.Setup(x => x.ViewModels).Returns(ViewModels);
        }

        [Test]
        public void GetComponent_when_type_is_component_the_just_return()
        {

            var output = blockType.Object.GetComponent(config.Object);

            Assert.AreEqual(blockType.Object, output);
        }

        [Test, Ignore("Moq doesn't support properties")]
        public void GetComponent_when_sub_is_child_object_then_return_parent()
        {
            SetupPropertyInfo(blockType, subBlockType);

            var output = subBlockType.Object.GetComponent(config.Object);

            Assert.AreEqual(blockType.Object, output);
        }

        /*[Test]
        public void GetComponent_when_sub_is_child_list_of_objects_then_return_parent()
        {
            SetupPropertyInfo(blockType, subListBlockType);

            var output = subBlockType.GetComponent(config);

            Assert.AreEqual(blockType, output);
        }

        [Test]
        public void GetComponent_when_sub_is_grandchild_object_then_return_grandparent()
        {
            SetupPropertyInfo(blockType, parentBlockType);
            SetupPropertyInfo(parentBlockType, subBlockType);

            var output = subBlockType.GetComponent(config);

            Assert.AreEqual(blockType, output);
        }

        [Test]
        public void GetComponent_when_sub_is_grandchild_list_of_objects_then_return_grandparent()
        {
            SetupPropertyInfo(blockType, parentBlockType);
            SetupPropertyInfo(parentBlockType, subListBlockType);

            var output = subBlockType.GetComponent(config);

            Assert.AreEqual(blockType, output);
        }

        [Test]
        public void GetComponent_when_parent_is_block_then_return_null()
        {
            SetupPropertyInfo(blockType, subExternalBlockType);

            var output = subBlockType.GetComponent(config);

            Assert.IsNull(output);
        }*/

        /*public PropertyInfo SetupPropertyInfo(Type parent, Type child)
        {
            var propertyInfo = MockRepository.GenerateStub<PropertyInfo>();
            propertyInfo.Stub(x => x.PropertyType).Return(child);

            parent.Stub(x => x.GetProperties()).Return(new PropertyInfo[] { propertyInfo });

            return propertyInfo;
        }*/

        public PropertyInfo SetupPropertyInfo(Mock<Type> parent, Mock<Type> child)
        {
            var propertyInfo = new Moq.Mock<PropertyInfo>();
            propertyInfo.Setup(x => x.PropertyType).Returns(child.Object);

            parent.Setup(x => x.GetProperties()).Returns(new PropertyInfo[] { propertyInfo.Object });

            return propertyInfo.Object;
        }

    }
}
