using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;

namespace YAXL.Common.Mvvm
{
    public static class SimpleIocHelpers
    {
        public static T TryGetInstance<T>(this SimpleIoc ioc) where T : class
            => ioc.IsRegistered<T>() ? ioc.GetInstance<T>() : null;

        public static void EnsureRegister<T>(this SimpleIoc ioc, T model)
            where T : class
        {
            if (ioc.IsRegistered<T>())
            {
                var existing = SimpleIoc.Default.GetInstance<T>();
                if (existing is ICleanup)
                    ((ICleanup)existing).Cleanup();
                ioc.Unregister<T>();
            }

            if (model != null)
                ioc.Register(() => model);
        }
    }
}
