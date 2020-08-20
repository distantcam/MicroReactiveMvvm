using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;

namespace MicroReactiveMVVM
{
    internal abstract class AbstractCommand<T> : ICommand<T>
    {
        readonly Func<T, bool>? canExecuteMethod;
        readonly SemaphoreSlim isExecuting = new SemaphoreSlim(1);
        readonly Subject<Unit> canExecuteObservable;
        EventHandler? canExecuteChanged;

        public AbstractCommand(Func<T, bool>? canExecuteMethod = null)
        {
            this.canExecuteMethod = canExecuteMethod;

            canExecuteObservable = new Subject<Unit>();
            CanExecuteObservable = canExecuteObservable.AsObservable();
            CanExecuteObservable.ObserveOnMain()
                .Subscribe(args =>
                {
                    canExecuteChanged?.Invoke(this, EventArgs.Empty);
                });
        }

        event EventHandler System.Windows.Input.ICommand.CanExecuteChanged
        {
            add
            {
                EventHandler? handler2;
                var newEvent = canExecuteChanged;
                do
                {
                    handler2 = newEvent;
                    var handler3 = (EventHandler)Delegate.Combine(handler2, value);
                    Interlocked.CompareExchange(ref canExecuteChanged, handler3, handler2);
                } while (newEvent != handler2);
            }
            remove
            {
                EventHandler? handler2;
                var newEvent = canExecuteChanged;
                do
                {
                    handler2 = newEvent;
                    var handler3 = (EventHandler)Delegate.Remove(handler2, value);
                    Interlocked.CompareExchange(ref canExecuteChanged, handler3, handler2);
                } while (newEvent != handler2);
            }
        }

        public IObservable<Unit> CanExecuteObservable { get; }

        public abstract void Execute(T obj);

        bool System.Windows.Input.ICommand.CanExecute(object parameter) => CanExecute((T)parameter);

        void System.Windows.Input.ICommand.Execute(object parameter) => ((ICommand<T>)this).Execute((T)parameter);

        public bool CanExecute(T parameter)
        {
            if (isExecuting.CurrentCount == 0)
                return false;
            if (canExecuteMethod == null)
                return true;

            return canExecuteMethod(parameter);
        }

        public void RaiseCanExecuteChanged() => canExecuteObservable.OnNext(Unit.Default);

        protected IDisposable StartExecuting()
        {
            isExecuting.Wait();
            RaiseCanExecuteChanged();

            return Disposable.Create(() =>
            {
                isExecuting.Release();
                RaiseCanExecuteChanged();
            });
        }
    }
}
