namespace MicroReactiveMVVM
{
    public class PropertyChangingData
    {
        public PropertyChangingData(string propertyName, object? value)
        {
            PropertyName = propertyName;
            Value = value;
        }

        public string PropertyName { get; }
        public object? Value { get; }
    }

    public class PropertyChangingData<TProperty>
    {
        public PropertyChangingData(string propertyName, TProperty value)
        {
            PropertyName = propertyName;
            Value = value;
        }

        public string PropertyName { get; }
        public TProperty Value { get; }

        public static implicit operator PropertyChangingData(PropertyChangingData<TProperty> data) =>
            new PropertyChangingData(data.PropertyName, data.Value);

        public static explicit operator PropertyChangingData<TProperty>(PropertyChangingData data) =>
            new PropertyChangingData<TProperty>(data.PropertyName, (TProperty)data.Value!);
    }
}
