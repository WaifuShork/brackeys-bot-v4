using System;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace BrackeysBot.SlashModules
{
    public sealed class TestModule : SlashCommandModule
    {
        [SlashCommand("ping", "A ping command using slash commands :D")]
        public async Task PingAsync(InteractionContext ctx)
        {
            var embed = new DiscordEmbedBuilder()
                .WithColor(DiscordColor.Aquamarine)
                .WithTitle("Pong! :ping_pong:")
                .AddField("WS Latency", $"{ctx.Client.Ping}ms")
                .WithTimestamp(DateTime.UtcNow);

            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, 
                new DiscordInteractionResponseBuilder()
                    .AddEmbed(embed.Build()));
        }
    }
}