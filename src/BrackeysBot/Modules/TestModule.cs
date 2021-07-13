using System.Threading.Tasks;
using BrackeysBot.Configuration;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace BrackeysBot.Modules
{
    [RequireGuild]
    public class TestModule : BaseCommandModule
    {
        public BotConfiguration Configuration { get; set; }

        [Command("prefix")]
        public async Task GetPrefix(CommandContext ctx)
        {
            await ctx.RespondAsync($"Current prefix is: {Configuration.Prefix}");
        }
    }
}