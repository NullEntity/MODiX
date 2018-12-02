using System;
using Moq;

namespace Modix.Bot.Test.Support
{
    public class CommandHelperServiceProvider : IServiceProvider
    {
        private readonly IServiceProvider _serviceProvider;

        public CommandHelperServiceProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public object GetService(Type serviceType)
        {
            var providedService = _serviceProvider.GetService(serviceType);
            if (providedService != null)
                return providedService;

            var t = typeof(Mock<>).MakeGenericType(serviceType);
            var obj = t.GetProperty("Object", serviceType);
            var defaultValueProvider = t.GetProperty("DefaultValueProvider");

            var instance = Activator.CreateInstance(t);
            defaultValueProvider.SetValue(instance, DefaultValueProvider.Mock);

            return obj.GetValue(instance);
        }
    }
}
