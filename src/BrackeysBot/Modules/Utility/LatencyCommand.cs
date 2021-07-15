using System;
using System.Threading.Tasks;
using DSharpPlusNextGen.CommandsNext;
using DSharpPlusNextGen.CommandsNext.Attributes;
using DSharpPlusNextGen.Entities;

namespace BrackeysBot.Modules
{
    public sealed partial class UtilityModule
    {
        [Command("test")]
        public Task TestCommandAsync(CommandContext context)
        {
            return context.RespondAsync(builder =>
            {
                var button1 = new EventDrivenDiscordButtonComponent("Button 1", context.Client);
                var button2 = new EventDrivenDiscordButtonComponent("Button 2", context.Client);

                button1.Clicked += (_, _) => context.Channel.SendMessageAsync("Button 1 was clicked");
                button2.Clicked += (_, _) => context.Channel.SendMessageAsync("Button 2 was clicked");

                builder.AddComponents(button1, button2);
                builder.Content = "Try a button";
            });
        }
        
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