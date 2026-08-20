using SteamKit2.Internal;
using SteamPrefill.Handlers;
using Xunit;

namespace SteamPrefill.Test
{
    public sealed class AppInfoHandlerTests
    {
        [Fact]
        public void FilterGamesPlayedWithinDays_IncludesBoundaryAndExcludesOlderOrNeverPlayedGames()
        {
            var now = DateTimeOffset.FromUnixTimeSeconds(2_000_000_000);
            var cutoff = now.AddDays(-365).ToUnixTimeSeconds();
            var games = new List<CPlayer_GetOwnedGames_Response.Game>
            {
                new() { appid = 1, rtime_last_played = (uint)now.ToUnixTimeSeconds() },
                new() { appid = 2, rtime_last_played = (uint)cutoff },
                new() { appid = 3, rtime_last_played = (uint)(cutoff - 1) },
                new() { appid = 4, rtime_last_played = 0 }
            };

            var result = AppInfoHandler.FilterGamesPlayedWithinDays(games, 365, now);

            Assert.Equal(new[] { 1, 2 }, result.Select(game => game.appid));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void FilterGamesPlayedWithinDays_RejectsNonPositiveDays(int days)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                AppInfoHandler.FilterGamesPlayedWithinDays(Array.Empty<CPlayer_GetOwnedGames_Response.Game>(), days, DateTimeOffset.UtcNow));
        }
    }
}
