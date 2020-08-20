using System;

namespace MicroReactiveMVVM
{
    public interface IObservablePropertyChanging
    {
        IObservable<PropertyChangingData> Changing { get; }
    }
}