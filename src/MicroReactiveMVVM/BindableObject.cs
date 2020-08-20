using System;
using System.Collections;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;

namespace MicroReactiveMVVM
{
    public class BindableObject : IObservablePropertyChanged, IObservablePropertyChanging, IObservableDataErrorInfo, IDisposable, INotifyPropertyChanged, INotifyPropertyChanging, INotifyDataErrorInfo
    {
        private long changeNotificationSuppressionCount;

        private Subject<PropertyChangedData> changed;
        private Subject<PropertyChangingData> changing;
        private Subject<DataErrorChanged> errorChanged;

        readonly IObservable<PropertyChangedData> whenChanged;
        readonly IObservable<PropertyChangingData> whenChanging;
        readonly IObservable<DataErrorChanged> whenErrorChanged;

        private volatile int disposeSignaled;

        private PropertyChangedEventHandler? propertyChanged;
        private PropertyChangingEventHandler? propertyChanging;

        private readonly ConcurrentDictionary<string, string> errors = new ConcurrentDictionary<string, string>();
        private EventHandler<DataErrorsChangedEventArgs>? errorsChanged;

        public BindableObject()
        {
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
                    if (string.IsNullOrEmpty(args.Error))
                    {
                        errors.TryRemove(args.PropertyName, out _);
                    }
                    else
                    {
                        errors.AddOrUpdate(args.PropertyName, args.Error, (_, __) => args.Error);
                    }
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

        protected virtual void OnPropertyChanged(string propertyName, object before, object after)
        {
            if (ChangeNotificationEnabled)
                changed.OnNext(new PropertyChangedData(propertyName, before, after));
        }

        protected virtual void OnPropertyChanging(string propertyName, object before)
        {
            if (ChangeNotificationEnabled)
                changing.OnNext(new PropertyChangingData(propertyName, before));
        }

        public void SetDataError(string propertyName, string error) => errorChanged.OnNext(new DataErrorChanged(propertyName, error));

        public void ResetDataError(string propertyName) => errorChanged.OnNext(new DataErrorChanged(propertyName, ""));

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

        IEnumerable INotifyDataErrorInfo.GetErrors(string propertyName) =>
            errors.TryGetValue(propertyName, out string value) ? value : "";

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
