using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Linq;
using System.Reflection;

namespace MicroReactiveMVVM
{
    public static class ObservableExtensions
    {
        public static ObservableCollection<T> ToCollection<T>(this IObservable<T> source)
        {
            var collection = new ObservableCollection<T>();
            source.Subscribe(t => collection.Add(t));
            return collection;
        }

        public static IObservable<PropertyChangedData> WhenPropertyChanged(this IObservablePropertyChanged changed, string propertyName)
            => changed.Changed.Where(p => p.PropertyName == propertyName);

        public static IObservable<PropertyChangedData<TProperty>> WhenPropertyChanged<TProperty>(this IObservablePropertyChanged changed, string propertyName)
            => changed.Changed.Where(p => p.PropertyName == propertyName).Select(data => (PropertyChangedData<TProperty>)data);

        public static IObservable<PropertyChangedData<TProperty>> WhenPropertyChanged<TObservable, TProperty>(this TObservable changed, Expression<Func<TObservable, TProperty>> property)
            where TObservable : IObservablePropertyChanged
            => WhenPropertyChanged<TProperty>(changed, GetPropertyName(property));

        public static IObservable<PropertyChangedData> WhenPropertiesChanged(this IObservablePropertyChanged changed, params string[] propertyNames)
            => changed.Changed.Where(p => propertyNames.Contains(p.PropertyName));

        public static IObservable<PropertyChangedData<TProperty>> WhenPropertiesChanged<TProperty>(this IObservablePropertyChanged changed, params string[] propertyNames)
            => changed.Changed.Where(p => propertyNames.Contains(p.PropertyName)).Select(data => (PropertyChangedData<TProperty>)data);

        public static IObservable<PropertyChangedData<TProperty>> WhenPropertiesChanged<TObservable, TProperty>(this TObservable changed, params Expression<Func<TObservable, TProperty>>[] propertiesNames)
            where TObservable : IObservablePropertyChanged
            => WhenPropertiesChanged<TProperty>(changed, propertiesNames.Select(GetPropertyName).ToArray());

        public static IObservable<PropertyChangingData> WhenPropertyChanging(this IObservablePropertyChanging changing, string propertyName)
            => changing.Changing.Where(p => p.PropertyName == propertyName);

        public static IObservable<PropertyChangingData<TProperty>> WhenPropertyChanging<TProperty>(this IObservablePropertyChanging changing, string propertyName)
            => changing.Changing.Where(p => p.PropertyName == propertyName).Select(data => (PropertyChangingData<TProperty>)data);

        public static IObservable<PropertyChangingData<TProperty>> WhenPropertyChanging<TObservable, TProperty>(this TObservable changing, Expression<Func<TObservable, TProperty>> property)
            where TObservable : IObservablePropertyChanging
            => WhenPropertyChanging<TProperty>(changing, GetPropertyName(property));

        public static IObservable<PropertyChangingData> WhenPropertiesChanging(this IObservablePropertyChanging changing, params string[] propertyNames)
            => changing.Changing.Where(p => propertyNames.Contains(p.PropertyName));

        public static IObservable<PropertyChangingData<TProperty>> WhenPropertiesChanging<TProperty>(this IObservablePropertyChanging changing, params string[] propertyNames)
            => changing.Changing.Where(p => propertyNames.Contains(p.PropertyName)).Select(data => (PropertyChangingData<TProperty>)data);

        public static IObservable<PropertyChangingData<TProperty>> WhenPropertiesChanging<TObservable, TProperty>(this TObservable changing, params Expression<Func<TObservable, TProperty>>[] propertiesNames)
            where TObservable : IObservablePropertyChanging
            => WhenPropertiesChanging<TProperty>(changing, propertiesNames.Select(GetPropertyName).ToArray());

        public static IObservable<PropertyChangedData<TProperty>> CastPropertyType<TProperty>(this IObservable<PropertyChangedData> observable)
            => observable.Select(data => (PropertyChangedData<TProperty>)data);

        public static IObservable<PropertyChangingData<TProperty>> CastPropertyType<TProperty>(this IObservable<PropertyChangingData> observable)
            => observable.Select(data => (PropertyChangingData<TProperty>)data);

        public static IObservable<PropertyChangedData<TProperty>> OfPropertyType<TProperty>(this IObservable<PropertyChangedData> observable)
            => observable.Where(data => data is PropertyChangedData<TProperty>).Select(data => (PropertyChangedData<TProperty>)data);

        public static IObservable<PropertyChangingData<TProperty>> OfPropertyType<TProperty>(this IObservable<PropertyChangingData> observable)
            => observable.Where(data => data is PropertyChangingData<TProperty>).Select(data => (PropertyChangingData<TProperty>)data);

        private static string GetPropertyName<TSource, TProperty>(Expression<Func<TSource, TProperty>> propertyLambda)
        {
            var type = typeof(TSource);
            if (!(propertyLambda.Body is MemberExpression member))
            {
                throw new ArgumentException($"Expression '{propertyLambda}' refers to a method, not a property");
            }
            if (!(member.Member is PropertyInfo propInfo))
            {
                throw new ArgumentException($"Expression '{propertyLambda}' refers to a field, not a property");
            }
            if (type != propInfo.ReflectedType && !type.IsSubclassOf(propInfo.ReflectedType))
            {
                throw new ArgumentException($"Expression '{propertyLambda}' refers to a property that is not from type {type}");
            }
            return propInfo.Name;
        }
    }
}
