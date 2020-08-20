using System;
using System.Reactive;

namespace MicroReactiveMVVM
{
    public interface ICommand<in T> : System.Windows.Input.ICommand, IRaiseCanExecuteChanged
    {
        void Execute(T obj);

        bool CanExecute(T obj);

        IObservable<Unit> CanExecuteObservable { get; }
    }
}