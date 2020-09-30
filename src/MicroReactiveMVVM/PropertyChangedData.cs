namespace MicroReactiveMVVM
{
    public class PropertyChangedData
    {
        public PropertyChangedData(string propertyName, object? value)
        {
            PropertyName = propertyName;
            Value = value;
        }

        public string PropertyName { get; }
        public object? Value { get; }
    }

    public class PropertyChangedData<TProperty>
    {
        public PropertyChangedData(string propertyName, TProperty value)
        {
            PropertyName = propertyName;
            Value = value;
        }

        public string PropertyName { get; }
        public TProperty Value { get; }

        public static implicit operator PropertyChangedData(PropertyChangedData<TProperty> data)
            => new PropertyChangedData(data.PropertyName, data.Value);

        public static explicit operator PropertyChangedData<TProperty>(PropertyChangedData data)
            => new PropertyChangedData<TProperty>(data.PropertyName, (TProperty)data.Value!);
    }
}
