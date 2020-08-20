using System;

namespace MicroReactiveMVVM
{
    public interface IObservablePropertyChanged
    {
        IObservable<PropertyChangedData> Changed { get; }
    }
}