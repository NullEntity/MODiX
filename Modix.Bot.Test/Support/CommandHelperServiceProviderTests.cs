using Microsoft.Extensions.DependencyInjection;
using Modix.Services.Core;
using Moq;
using NUnit.Framework;
using Shouldly;

namespace Modix.Bot.Test.Support
{
    [TestFixture]
    public class CommandHelperServiceProviderTests
    {
        [Test]
        public void ServiceProvider_WhenNotGivenBinding_ShouldMockServices()
        {
            var serviceProvider = new ServiceCollection().BuildServiceProvider();
            var sut = new CommandHelperServiceProvider(serviceProvider);
            var mock = sut.GetService<IAuthorizationService>();
            mock.ShouldNotBeNull();
        }

        [Test]
        public void ServiceProvider_WhenGivenBinding_ShouldReturnBoundService()
        {
            var authService = Mock.Of<IAuthorizationService>();
            var serviceProvider = new ServiceCollection()
                .AddSingleton(authService)
                .BuildServiceProvider();
            var sut = new CommandHelperServiceProvider(serviceProvider);

            var resolvedService = sut.GetService<IAuthorizationService>();

            resolvedService.ShouldBe(authService);
        }
    }
}
