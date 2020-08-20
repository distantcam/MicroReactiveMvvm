using System.Reactive.Concurrency;

namespace MicroReactiveMVVM
{
    public static class MvvmContext
    {
        static MvvmContext()
        {
            MainScheduler = DefaultScheduler.Instance;
            BackgroundScheduler = ThreadPoolScheduler.Instance;
        }

        public static IScheduler MainScheduler { get; set; }
        public static IScheduler BackgroundScheduler { get; set; }
    }
}
