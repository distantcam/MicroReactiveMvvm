using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;

namespace MicroReactiveMVVM
{
    public class ReactiveObject : IObservablePropertyChanged, IObservablePropertyChanging, IObservableDataErrorInfo, IDisposable, INotifyPropertyChanged, INotifyPropertyChanging, INotifyDataErrorInfo
    {
        private long changeNotificationSuppressionCount;

        private Subject<PropertyChangedData> changed;
        private Subject<PropertyChangingData> changing;
        private Subject<DataErrorChanged> errorChanged;

        private readonly IObservable<PropertyChangedData> whenChanged;
        private readonly IObservable<PropertyChangingData> whenChanging;
        private readonly IObservable<DataErrorChanged> whenErrorChanged;

        private volatile int disposeSignaled;

        private PropertyChangedEventHandler? propertyChanged;
        private PropertyChangingEventHandler? propertyChanging;

        private readonly ConcurrentDictionary<string, List<string>> errors = new ConcurrentDictionary<string, List<string>>();
        private EventHandler<DataErrorsChangedEventArgs>? errorsChanged;

        private readonly CompositeDisposable disposables;

        public ReactiveObject()
        {
            disposables = new CompositeDisposable();

            changed = new Subject<PropertyChangedData>();
            whenChanged = changed.AsObservable();
            whenChanged.ObserveOnMain()
                .Subscribe(args =>
                {
                    propertyChanged?.Invoke(this, new PropertyChangedEventArgs(args.PropertyName));
                });

            changing = new Subject<PropertyChangingData>();
            whenChanging = changing.AsObservable();
            whenChanging.ObserveOnMain()
                .Subscribe(args =>
                {
                    propertyChanging?.Invoke(this, new PropertyChangingEventArgs(args.PropertyName));
                });

            errorChanged = new Subject<DataErrorChanged>();
            whenErrorChanged = errorChanged.AsObservable();
            whenErrorChanged.ObserveOnMain()
                .Subscribe(args =>
                {

                    errorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(args.PropertyName));
                });
        }

        IObservable<PropertyChangedData> IObservablePropertyChanged.Changed => whenChanged;
        IObservable<PropertyChangingData> IObservablePropertyChanging.Changing => whenChanging;
        IObservable<DataErrorChanged> IObservableDataErrorInfo.ErrorsChanged => whenErrorChanged;

        public bool ChangeNotificationEnabled => Interlocked.Read(ref changeNotificationSuppressionCount) == 0L;

        public IDisposable SuppressNotifications()
        {
            Interlocked.Increment(ref changeNotificationSuppressionCount);
            return Disposable.Create(() => Interlocked.Decrement(ref changeNotificationSuppressionCount));
        }

        public virtual void Dispose()
        {
            if (Interlocked.Exchange(ref disposeSignaled, 1) != 0)
            {
                return;
            }
            if (!disposables.IsDisposed)
            {
                disposables.Dispose();
            }
            if (changing != null)
            {
                changing.OnCompleted();
                changing.Dispose();
            }
            if (changed != null)
            {
                changed.OnCompleted();
                changed.Dispose();
            }
            if (errorChanged != null)
            {
                errorChanged.OnCompleted();
                errorChanged.Dispose();
            }
        }

        public void AddDisposable(IDisposable item)
        {
            disposables.Add(item);
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (ChangeNotificationEnabled)
                changed.OnNext(new PropertyChangedData(this, propertyName));
        }

        protected virtual void OnPropertyChanging(string propertyName, object? before)
        {
            if (ChangeNotificationEnabled)
                changing.OnNext(new PropertyChangingData(propertyName, before));
        }

        public void SetDataError(string propertyName, string error)
        {
            if (string.IsNullOrEmpty(error))
            {
                errors.TryRemove(propertyName, out _);
            }
            else
            {
                errors.AddOrUpdate(propertyName, new List<string> { error }, (_, list) =>
                {
                    list.Add(error);
                    return list;
                });
            }
            errorChanged.OnNext(new DataErrorChanged(propertyName, error));
        }

        public void ResetDataError(string propertyName)
        {
            errors.TryRemove(propertyName, out _);
            errorChanged.OnNext(new DataErrorChanged(propertyName, ""));
        }

        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            add
            {
                PropertyChangedEventHandler? handler2;
                var newEvent = propertyChanged;
                do
                {
                    handler2 = newEvent;
                    var handler3 = (PropertyChangedEventHandler)Delegate.Combine(handler2, value);
                    Interlocked.CompareExchange(ref propertyChanged, handler3, handler2);
                } while (newEvent != handler2);
            }
            remove
            {
                PropertyChangedEventHandler? handler2;
                var newEvent = propertyChanged;
                do
                {
                    handler2 = newEvent;
                    var handler3 = (PropertyChangedEventHandler)Delegate.Remove(handler2, value);
                    Interlocked.CompareExchange(ref propertyChanged, handler3, handler2);
                } while (newEvent != handler2);
            }
        }

        event PropertyChangingEventHandler INotifyPropertyChanging.PropertyChanging
        {
            add
            {
                PropertyChangingEventHandler? handler2;
                var newEvent = propertyChanging;
                do
                {
                    handler2 = newEvent;
                    var handler3 = (PropertyChangingEventHandler)Delegate.Combine(handler2, value);
                    Interlocked.CompareExchange(ref propertyChanging, handler3, handler2);
                } while (newEvent != handler2);
            }
            remove
            {
                PropertyChangingEventHandler? handler2;
                var newEvent = propertyChanging;
                do
                {
                    handler2 = newEvent;
                    var handler3 = (PropertyChangingEventHandler)Delegate.Remove(handler2, value);
                    Interlocked.CompareExchange(ref propertyChanging, handler3, handler2);
                } while (newEvent != handler2);
            }
        }

        bool INotifyDataErrorInfo.HasErrors => errors.Count != 0;

        IEnumerable? INotifyDataErrorInfo.GetErrors(string propertyName) =>
            errors.TryGetValue(propertyName, out var value) ? value : null;

        event EventHandler<DataErrorsChangedEventArgs> INotifyDataErrorInfo.ErrorsChanged
        {
            add
            {
                EventHandler<DataErrorsChangedEventArgs>? handler2;
                var newEvent = errorsChanged;
                do
                {
                    handler2 = newEvent;
                    var handler3 = (EventHandler<DataErrorsChangedEventArgs>)Delegate.Combine(handler2, value);
                    Interlocked.CompareExchange(ref errorsChanged, handler3, handler2);
                } while (newEvent != handler2);
            }
            remove
            {
                EventHandler<DataErrorsChangedEventArgs>? handler2;
                var newEvent = errorsChanged;
                do
                {
                    handler2 = newEvent;
                    var handler3 = (EventHandler<DataErrorsChangedEventArgs>)Delegate.Remove(handler2, value);
                    Interlocked.CompareExchange(ref errorsChanged, handler3, handler2);
                } while (newEvent != handler2);
            }
        }
    }
}
