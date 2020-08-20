using System;
using System.Reactive.Linq;

namespace MicroReactiveMVVM
{
    public static class ReactiveExtensions
    {
        public static IObservable<TSource> ObserveOnMain<TSource>(this IObservable<TSource> source)
            => source.ObserveOn(MvvmContext.MainScheduler);

        public static IObservable<TSource> ObserveOnBackground<TSource>(this IObservable<TSource> source)
            => source.ObserveOn(MvvmContext.BackgroundScheduler);

        public static IObservable<TSource> SubscribeOnMain<TSource>(this IObservable<TSource> source)
            => source.SubscribeOn(MvvmContext.MainScheduler);

        public static IObservable<TSource> SubscribeOnBackground<TSource>(this IObservable<TSource> source)
            => source.SubscribeOn(MvvmContext.BackgroundScheduler);
    }
}
