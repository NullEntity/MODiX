using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Modix.Data.Models.Core;
using Modix.Modules;
using Modix.Services.Core;
using Moq;
using NUnit.Framework;
using Shouldly;

namespace Modix.Bot.Test
{
    [TestFixture]
    public class AuthorizationModuleTests
    {
        [Test]
        public async Task ClaimsCommand_GivenNull_RepliesWithBotUserClaims()
        {
            var cmdHelper = new CommandHelper();
            cmdHelper.Services.AddSingleton(
                Mock.Of<IAuthorizationService>(s => s.CurrentClaims == new[] {AuthorizationClaim.ModerationWarn}));

            var result = await cmdHelper.GetCommandResponse<AuthorizationModule>("auth claims");
            result.ShouldNotBeNull();
            result.ShouldContain("warn");
        }

        [Test]
        public async Task ClaimsCommand_GivenUser_RepliesWithUserClaims()
        {
            var cmdHelper = new CommandHelper();
            cmdHelper.Services.Add

            cmdHelper.Services.AddSingleton(
                Mock.Of<IAuthorizationService>(s => s.CurrentClaims == new[] {AuthorizationClaim.ModerationWarn}));
        }
    }
}
