using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Shouldly;

namespace Modix.Bot.Test
{
    internal class CommandHelper
    {
        public IServiceCollection Services { get; set; } = new ServiceCollection();

        public async Task<string> GetCommandResponse<T>(string messageContent)
            where T : ModuleBase
        {
            var provider = Services.BuildServiceProvider();
            var cs = new CommandService();
            await cs.AddModuleAsync<T>(provider);

            var client = Mock.Of<IDiscordClient>();
            var messageMock = new Mock<IUserMessage> {DefaultValue = DefaultValue.Mock};
            messageMock.Setup(m => m.Content).Returns(messageContent);
            messageMock.Setup(m => m.Channel).Returns(Mock.Of<ITextChannel>());
            var message = messageMock.Object;

            string result = null;
            Mock.Get(message.Channel)
                .Setup(c => c.SendMessageAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<Embed>(), It.IsAny<RequestOptions>()))
                .Returns(() => Task.FromResult(Mock.Of<IUserMessage>()))
                .Callback<string, bool, Embed, RequestOptions>((s, _, __, ___) => result = s);

            var ctx = new CommandContext(client, message);

            var commandResult = await cs.ExecuteAsync(ctx, 0, provider);
            if (commandResult is SearchResult sr)
                throw new ArgumentException($"Command \"{messageContent}\" matched {sr.Commands?.Count ?? 0} commands");

            var executeResult = (ExecuteResult)commandResult;

            executeResult.IsSuccess.ShouldBeTrue(() => "Command returned following exception:\n" + executeResult.Exception);
            return result;
        }
    }
}
