using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;

namespace DBFilesViewer
{
    /// <summary>
    /// A generic wrapper object used to map fields to fake properties.
    /// Used in conjunction with PropertyGrid.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PropertyGridProxy<T> : DynamicObject, ICustomTypeDescriptor
    {
        private readonly Dictionary<string, object> _fields;
        private T _instance;

        public PropertyGridProxy(T instance)
        {
            _fields = new Dictionary<string, object>();
            _instance = instance;

            foreach (var fieldInfo in instance.GetType().GetFields())
                _fields[fieldInfo.Name] = fieldInfo.GetValue(instance);
        }

        #region Implementation of ICustomTypeDescriptor
        public PropertyDescriptorCollection GetProperties()
        {
            var properties = _fields.Select(pair => new DynamicPropertyDescriptor(this, pair.Key, pair.Value.GetType(), new Attribute[0]));
            return new PropertyDescriptorCollection(properties.Cast<PropertyDescriptor>().ToArray(), true);
        }

        public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            var properties = _fields.Select(pair => new DynamicPropertyDescriptor(this, pair.Key, pair.Value.GetType(), attributes));
            return new PropertyDescriptorCollection(properties.Cast<PropertyDescriptor>().ToArray(), true);
        }

        public object GetPropertyOwner(PropertyDescriptor pd) => this;
        public AttributeCollection GetAttributes() => TypeDescriptor.GetAttributes(_instance, true);
        public string GetClassName() => TypeDescriptor.GetClassName(_instance, true);
        public string GetComponentName() => TypeDescriptor.GetComponentName(_instance, true);
        public TypeConverter GetConverter() => TypeDescriptor.GetConverter(_instance, true);
        public EventDescriptor GetDefaultEvent() => TypeDescriptor.GetDefaultEvent(_instance, true);
        public PropertyDescriptor GetDefaultProperty() => TypeDescriptor.GetDefaultProperty(_instance, true);
        public object GetEditor(Type editorBaseType) => TypeDescriptor.GetEditor(_instance, editorBaseType, true);
        public EventDescriptorCollection GetEvents(Attribute[] attributes) => TypeDescriptor.GetEvents(_instance, attributes, true);
        public EventDescriptorCollection GetEvents() => TypeDescriptor.GetEvents(_instance, true);
        #endregion

        private class DynamicPropertyDescriptor : PropertyDescriptor
        {
            private readonly PropertyGridProxy<T> _proxyObject;

            public DynamicPropertyDescriptor(PropertyGridProxy<T> businessObject, string propertyName, Type propertyType, Attribute[] propertyAttributes)
                : base(propertyName, propertyAttributes)
            {
                _proxyObject = businessObject;
                PropertyType = propertyType;
            }

            public override bool CanResetValue(object component) => false;

            public override object GetValue(object component)
            {
                return _proxyObject._fields[Name];
            }

            public override void ResetValue(object component) { }
            public override void SetValue(object component, object value) => _proxyObject._fields[Name] = value;
            public override bool ShouldSerializeValue(object component) => false;
            public override Type ComponentType => typeof(PropertyGridProxy<T>);
            public override bool IsReadOnly => false;

            public override Type PropertyType { get; }
        }
    }
}
