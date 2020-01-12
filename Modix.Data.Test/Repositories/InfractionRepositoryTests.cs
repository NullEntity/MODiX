using System;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Modix.Data.Models;
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

        [Test]
        public async Task SearchSummaries_Works()
        {
            var options = new DbContextOptionsBuilder<ModixContext>()
                .UseNpgsql("Server=127.0.0.1;Port=5432;Database=modix;User Id=postgres;Password=matrix;");

            await using var modixContext = new ModixContext(options.Options);
            await modixContext.Database.EnsureCreatedAsync();

            var moderationActionEventHandlers = Enumerable.Empty<IModerationActionEventHandler>();
            var infractionEventHandlers = Enumerable.Empty<IInfractionEventHandler>();

            var infractionRepository = new InfractionRepository(modixContext, moderationActionEventHandlers,
                infractionEventHandlers);

            var searchCriteria = new InfractionSearchCriteria
            {
//                GuildId = 123,
                Subject = "Scratch#4334"
            };
            var results = await infractionRepository.SearchSummariesAsync(searchCriteria,
                new SortingCriteria[0]);
        }
    }
}
