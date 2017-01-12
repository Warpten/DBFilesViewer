using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Linq;
using DBFilesClient.NET.Types;
using RelDBC.FileSystem;

namespace RelDBC.Utils
{
    internal class ForeignKeyConverter : ExpandableObjectConverter
    {
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
                return value.ToString();

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }

    
    [TypeConverter(typeof(ForeignKeyConverter))]
    public class ForeignKey<T, U> : IObjectType<U>, ICustomTypeDescriptor where T : class, new() where U : struct
    {
        private readonly Dictionary<string, object> _fields;

        public ForeignKey(U underlyingValue) : base(underlyingValue)
        {
            _fields = new Dictionary<string, object>();
        }

        // Not visible through PropertyGrid because of the implementation of ICustomTypeDescriptor
        // (Perfectly intended)
        public T Value
        {
            get
            {
                var store = Manager.GetStore<T>();
                try
                {
                    return store?[Convert.ToInt32(Key)];
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        //! TODO This tremendously slows down ObjectListView because it inherently uses
        //! TODO `ToString() to display cell values.
        //! TODO ObjectListView needs to call another method which always returns
        //! TODO `Key.ToString()`.
        //! TODO This would enable stuff like ToGridString() => $"{Typename} ({Key})"
        // public override string ToString()
        // {
        //     return Value?.ToString() ?? Key.ToString();
        // }

        #region Implementation of ICustomTypeDescriptor
        public object GetPropertyOwner(PropertyDescriptor pd) => this;
        public AttributeCollection GetAttributes() => TypeDescriptor.GetAttributes(Value, true);
        public string GetClassName() => TypeDescriptor.GetClassName(Value, true);
        public string GetComponentName() => TypeDescriptor.GetComponentName(Value, true);
        public TypeConverter GetConverter() => TypeDescriptor.GetConverter(Value, true);
        public EventDescriptor GetDefaultEvent() => TypeDescriptor.GetDefaultEvent(Value, true);
        public PropertyDescriptor GetDefaultProperty() => TypeDescriptor.GetDefaultProperty(Value, true);
        public object GetEditor(Type editorBaseType) => TypeDescriptor.GetEditor(Value, editorBaseType, true);
        public EventDescriptorCollection GetEvents(Attribute[] attributes) => TypeDescriptor.GetEvents(Value, attributes, true);
        public EventDescriptorCollection GetEvents() => TypeDescriptor.GetEvents(Value, true);

        public PropertyDescriptorCollection GetProperties() => GetProperties(new Attribute[0]);

        public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            // Populate the cache now
            // Cannot be done at construction time because of the order in which files load - which cannot be predicted.
            if (_fields.Count == 0)
                foreach (var fieldInfo in typeof(T).GetFields())
                    _fields[fieldInfo.Name] = fieldInfo.GetValue(Value);

            var properties = _fields.Select(pair => new DynamicPropertyDescriptor(this, pair.Key, pair.Value.GetType(), attributes));
            return new PropertyDescriptorCollection(properties.ToArray(), true);
        }
        #endregion

        private class DynamicPropertyDescriptor : PropertyDescriptor
        {
            private readonly ForeignKey<T, U> _proxyObject;

            public DynamicPropertyDescriptor(ForeignKey<T, U> businessObject, string propertyName, Type propertyType, Attribute[] propertyAttributes)
                : base(propertyName, propertyAttributes)
            {
                _proxyObject = businessObject;
                PropertyType = propertyType;
            }

            public override bool CanResetValue(object component) => false;

            public override object GetValue(object component)
            {
                var value = _proxyObject._fields[Name];
                return value;
            }

            public override void ResetValue(object component) { }
            public override void SetValue(object component, object value) => _proxyObject._fields[Name] = value;
            public override bool ShouldSerializeValue(object component) => false;
            public override Type ComponentType => typeof(PropertyGridProxy);
            public override bool IsReadOnly => false;

            public override Type PropertyType { get; }
        }
    }
}
