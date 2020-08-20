using MicroReactiveMVVM;
using System;

namespace MicroReactiveMVVM
{
    public interface IObservableDataErrorInfo
    {
        IObservable<DataErrorChanged> ErrorsChanged { get; }
    }
}