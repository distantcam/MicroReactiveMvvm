using System;
using System.Reflection;

namespace MicroReactiveMVVM
{
    public class PropertyChangedData
    {
        readonly Lazy<MethodInfo> propertyGetter;

        public PropertyChangedData(object source, string propertyName)
        {
            Source = source;
            PropertyName = propertyName;

            propertyGetter = new Lazy<MethodInfo>(() => Source.GetType().GetProperty(PropertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy).GetGetMethod(true));
        }

        public object Source { get; }
        public string PropertyName { get; }

        public object? Value => propertyGetter.Value.Invoke(Source, null);
    }

    public class PropertyChangedData<TProperty>
    {
        readonly Lazy<MethodInfo> propertyGetter;

        public PropertyChangedData(object source, string propertyName)
        {
            Source = source;
            PropertyName = propertyName;

            propertyGetter = new Lazy<MethodInfo>(() => Source.GetType().GetProperty(PropertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy).GetGetMethod(true));
        }

        public object Source { get; }
        public string PropertyName { get; }

        public TProperty Value => (TProperty)propertyGetter.Value.Invoke(Source, null);


        public static implicit operator PropertyChangedData(PropertyChangedData<TProperty> data)
            => new PropertyChangedData(data.Source, data.PropertyName);

        public static explicit operator PropertyChangedData<TProperty>(PropertyChangedData data)
            => new PropertyChangedData<TProperty>(data.Source, data.PropertyName);
    }
}
