using System;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Modix.Data.Models;
using Modix.Data.Models.Core;
using Modix.Data.Models.Moderation;
using NUnit.Framework;
using NSubstitute;
using Shouldly;

using Modix.Data.Repositories;
using Modix.Data.Utilities;

namespace Modix.Data.Test.Repositories
{
    [TestFixture]
    public class InfractionRepositoryTests
    {
        [Test]
        public void Constructor_Always_InvokesBaseConstructor()
        {
            var modixContext = Substitute.For<ModixContext>();
            var moderationActionEventHandlers = Enumerable.Empty<IModerationActionEventHandler>();
            var infractionEventHandlers = Enumerable.Empty<IInfractionEventHandler>();

            var uut = new InfractionRepository(modixContext, moderationActionEventHandlers, infractionEventHandlers);

            uut.ModixContext.ShouldBeSameAs(modixContext);
        }

        [Ignore("Manual test, requires db setup and appSettings in this project")]
        [TestCase("RubyNova", "RubyNova", 1234, "RubyNova")]
        [TestCase("RubyNova", "RubyNova", 1234, "Rub")]
        [TestCase("RubyNova", "RubyNova", 1234, "rub")]
        public async Task UserSearch_Integration_HappyPath(string nickname, string username, int discriminator, string search)
        {
            var configuration = CreateConfig();
            var connectionString = configuration.GetValue<string>(nameof(ModixConfig.DbConnection));
            var optionsBuilder = new DbContextOptionsBuilder<ModixContext>()
                .UseNpgsql(connectionString);

            await using var modixContext = new ModixContext(optionsBuilder.Options);
            await modixContext.Database.MigrateAsync();

            modixContext.ModerationActions.RemoveRange(modixContext.ModerationActions);
            modixContext.GuildUsers.RemoveRange(modixContext.GuildUsers);
            modixContext.Users.RemoveRange(modixContext.Users);
            await modixContext.SaveChangesAsync();

            var ruby = new GuildUserEntity
            {
                Nickname = nickname,
                User = new UserEntity
                {
                    Username = username,
                    Discriminator = discriminator.ToString()
                }
            };
            modixContext.GuildUsers.Add(ruby);
            await modixContext.SaveChangesAsync();

            var moderationActionEventHandlers = Enumerable.Empty<IModerationActionEventHandler>();
            var infractionEventHandlers = Enumerable.Empty<IInfractionEventHandler>();
            var infractionRepository = new InfractionRepository(modixContext, moderationActionEventHandlers, infractionEventHandlers);

            await infractionRepository.CreateAsync(new InfractionCreationData
            {
                SubjectId = ruby.UserId,
                CreatedById = ruby.UserId,
                GuildId = ruby.GuildId,
                Type = InfractionType.Warning,
                Reason = "big succ",
            });

            var searchCriteria = new InfractionSearchCriteria
            {
                Creator = search
            };
            var results = await infractionRepository.SearchSummariesAsync(searchCriteria);

            results.ShouldHaveSingleItem().CreateAction.CreatedBy.Id.ShouldBe(ruby.UserId);
        }

        private IConfigurationRoot CreateConfig()
        {
            const string DEVELOPMENT_ENVIRONMENT_VARIABLE = "ASPNETCORE_ENVIRONMENT";
            const string DEVELOPMENT_ENVIRONMENT_KEY = "Development";

            var environment = Environment.GetEnvironmentVariable(DEVELOPMENT_ENVIRONMENT_VARIABLE);

            var configBuilder = new ConfigurationBuilder()
                .AddJsonFile("developmentSettings.json", optional: true, reloadOnChange: false);

            if(environment is DEVELOPMENT_ENVIRONMENT_KEY)
            {
                configBuilder.AddUserSecrets<Program>();
            }

            return configBuilder.Build();
        }
    }
}
