using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using DBFilesClient.NET.Types;
using DBFilesViewer.Data.IO.Files;

namespace DBFilesViewer.Data.Structures.Types
{
    internal class ForeignKeyConverter : ExpandableObjectConverter
    {
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {            
            if (destinationType == typeof (string))
            {
                var valueType = value.GetType();
                if (valueType.IsGenericType)
                    return $"{valueType.GetGenericArguments()[0].Name} (#{value})";
                return $"{valueType.Name} (#{value})";
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }

    [TypeConverter(typeof (ForeignKeyConverter))]
    public class ForeignKey<T> : IObjectType<uint>, ICustomTypeDescriptor, IForeignKey where T : class, new()
    {
        /// <summary>
        /// This delegate is the basis for foreign keys implementation.
        /// 
        /// It performs the logic for finding the appropriate referenced
        /// entry.
        /// 
        /// Classes that inherit ForeignKey should override this delegate.
        /// </summary>
        protected virtual Func<uint, T> ValueGetter { get; } = key =>
        {
            try {
                return DBC.Get<T>()?[key];
            } catch (Exception) {
                return null;
            }
        };

        /// <summary>
        /// This property gives the name of the key column to display.
        /// In most cases, it does not need to be overriden, however there
        /// are situations where a foreign key does not map to the index
        /// column of a file, meaning this name is not valid anymore.
        /// </summary>
        protected virtual string KeyName { get; } = "ID";

        public ForeignKey(uint underlyingValue) : base(underlyingValue)
        {
        }

        // Not visible through PropertyGrid because of the implementation of ICustomTypeDescriptor
        // (Perfectly intended)
        private T _cachedValue;
        public T Value
        {
            get
            {
                if (_cachedValue != null)
                    return _cachedValue;

                _cachedValue = ValueGetter(Key);
                return _cachedValue;
            }
        }

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
            if (Value != null)
            {
                if (_properties != null)
                    return _properties;

                var collection = typeof (T).GetFields()
                    .Select(
                        fieldInfo => (PropertyDescriptor) new FieldDescriptor<T>(fieldInfo, fieldInfo.GetValue(Value), attributes)).ToList();

                if (DBC.Get<T>().HasIndexTable)
                    collection.Add(new FieldDescriptor<T>("ID", Key, null));

                _properties = new PropertyDescriptorCollection(collection.ToArray(), true);
                return _properties;
            }

            return null;
        }
        #endregion

        public Type ValueType { get; } = typeof(T);

        private PropertyDescriptorCollection _properties;
    }

    public interface IForeignKey
    {
        Type ValueType { get; }
    }

    public class FieldDescriptor<T> : PropertyDescriptor
    {
        private object _value;

        public FieldDescriptor(FieldInfo fieldInfo, object fieldValue, Attribute[] fieldAttributes)
            : base(fieldInfo.Name, fieldAttributes)
        {
            _value = fieldValue;
            PropertyType = fieldValue.GetType();
        }

        public FieldDescriptor(string fieldName, object fieldValue, Attribute[] fieldAttributes)
            : base(fieldName, fieldAttributes)
        {
            _value = fieldValue;
            PropertyType = fieldValue.GetType();
        }

        public override bool CanResetValue(object component) => false;

        public override object GetValue(object component) => _value;

        public override void ResetValue(object component) { }
        public override void SetValue(object component, object value) { }
        public override bool ShouldSerializeValue(object component) => false;
        public override Type ComponentType => typeof(PropertyGridProxy<T>);
        public override bool IsReadOnly => false;

        public override Type PropertyType { get; }
    }
}
