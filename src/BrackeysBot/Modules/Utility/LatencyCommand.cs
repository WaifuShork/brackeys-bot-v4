using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

namespace BrackeysBot.Modules
{
    public sealed partial class UtilityModule
    {
        [Command("ping"), Aliases("latency", "late")]
        [Description("Gets the current latency of the bot")]
        public async Task GetLatencyAsync(CommandContext ctx)
        {
            var timeStamp = ctx.Message.CreationTimestamp;

            var latency = ctx.Client.Ping;

            var color = latency >= 300 ? DiscordColor.DarkRed : DiscordColor.Green;
            var message = await ctx.Channel.SendMessageAsync("Checking...");
            
            var newLat = (message.CreationTimestamp - timeStamp).Milliseconds;
            
            await message.ModifyAsync(m => m
                .Embed = new DiscordEmbedBuilder()
                .WithColor(color)
                .WithTitle("Pong! :ping_pong:")
                .AddField("WS Latency", $"{latency}ms")
                .AddField("Client Latency", $"{newLat}ms")
                .Build());
        }
    }
}