using System;
using Microsoft.Extensions.DependencyInjection;

namespace Modix.Bot.Test
{
    public class AutoMockingServiceProviderFactory : IServiceProviderFactory<IServiceCollection>
    {
        public IServiceCollection CreateBuilder(IServiceCollection services) => services;

        public IServiceProvider CreateServiceProvider(IServiceCollection containerBuilder)
        {
            return null;
        }
    }
}
