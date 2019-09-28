using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Rhino.Mocks;
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

        [SetUp]
        public void Setup()
        {
            blockType = MockRepository.GenerateStub<Type>();
            blockType.Stub(x => x.Name).Return("vmBlock_Test");

            subBlockType = MockRepository.GenerateStub<Type>();
            subBlockType.Stub(x => x.Name).Return("vmSub_Test");

            subListBlockType = MockRepository.GenerateStub<Type>();
            subListBlockType.Stub(x => x.IsGenericType).Return(true);
            subListBlockType.Stub(x => x.GenericTypeArguments).Return(new Type[] { subBlockType });

            parentBlockType = MockRepository.GenerateStub<Type>();
            parentBlockType.Stub(x => x.Name).Return("vmSub_Parent");

            subExternalBlockType = MockRepository.GenerateStub<Type>();
            subExternalBlockType.Stub(x => x.Name).Return("vmBlock_External");

            ViewModels = new List<Type>();
            ViewModels.Add(blockType);

            var config = MockRepository.GeneratePartialMock<YuzuConfiguration>();
            config.Stub(x => x.ViewModels).Return(ViewModels);

            Yuzu.Reset();
            Yuzu.Initialize(config);
        }

        [Test]
        public void GetComponent_when_type_is_component_the_just_return()
        {

            var output = blockType.GetComponent();

            Assert.AreEqual(blockType, output);
        }

        [Test]
        public void GetComponent_when_sub_is_child_object_then_return_parent()
        {
            SetupPropertyInfo(blockType, subBlockType);

            var output = subBlockType.GetComponent();

            Assert.AreEqual(blockType, output);
        }

        [Test]
        public void GetComponent_when_sub_is_child_list_of_objects_then_return_parent()
        {
            SetupPropertyInfo(blockType, subListBlockType);

            var output = subBlockType.GetComponent();

            Assert.AreEqual(blockType, output);
        }

        [Test]
        public void GetComponent_when_sub_is_grandchild_object_then_return_grandparent()
        {
            SetupPropertyInfo(blockType, parentBlockType);
            SetupPropertyInfo(parentBlockType, subBlockType);

            var output = subBlockType.GetComponent();

            Assert.AreEqual(blockType, output);
        }

        [Test]
        public void GetComponent_when_sub_is_grandchild_list_of_objects_then_return_grandparent()
        {
            SetupPropertyInfo(blockType, parentBlockType);
            SetupPropertyInfo(parentBlockType, subListBlockType);

            var output = subBlockType.GetComponent();

            Assert.AreEqual(blockType, output);
        }

        [Test]
        public void GetComponent_when_parent_is_block_then_return_null()
        {
            SetupPropertyInfo(blockType, subExternalBlockType);

            var output = subBlockType.GetComponent();

            Assert.IsNull(output);
        }

        public PropertyInfo SetupPropertyInfo(Type parent, Type child)
        {
            var propertyInfo = MockRepository.GenerateStub<PropertyInfo>();
            propertyInfo.Stub(x => x.PropertyType).Return(child);

            parent.Stub(x => x.GetProperties()).Return(new PropertyInfo[] { propertyInfo });

            return propertyInfo;
        }

    }
}
