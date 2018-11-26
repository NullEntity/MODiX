using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using Modix.Modules;
using Modix.Services.Moderation;
using Moq;
using NUnit.Framework;
using Shouldly;

namespace Modix.Bot.Test
{
    [TestFixture]
    public class AttachmentBlacklistModuleTests
    {
        [Test]
        public async Task AttachmentBlacklistCommand_Returns_BlacklistedExtensions()
        {
            var cmdHelper = new CommandHelper();

            var result = await cmdHelper.GetCommandResponse<AttachmentBlacklistModule>("attachment blacklist");

            result.ShouldNotBeNull();
            result.ShouldContain("Blacklisted Extensions");
            result.ShouldContain(".exe");
        }
    }
}
