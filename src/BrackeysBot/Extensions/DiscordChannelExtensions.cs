using System.Threading.Tasks;
using DSharpPlusNextGen.Entities;

namespace BrackeysBot.Extensions
{
    public static class DiscordChannelExtensions
    {
        public static async Task<DiscordMessage> SendErrorEmbedAsync(this DiscordChannel channel, string contents)
        {
            return await new DiscordEmbedBuilder()
                .WithTitle("Error")
                .WithColor(DiscordColor.Red)
                .WithDescription(contents)
                .BuildToChannelAsync(channel);
        }
        
        public static async Task<DiscordMessage> SendSuccessEmbedAsync(this DiscordChannel channel, string contents)
        {
            return await new DiscordEmbedBuilder()
                .WithTitle("Success")
                .WithColor(DiscordColor.Green)
                .WithDescription(contents)
                .BuildToChannelAsync(channel);
        }

        public static async Task<DiscordMessage> SendColoredEmbedAsync(this DiscordChannel channel, string contents, DiscordColor color)
        {
            return await new DiscordEmbedBuilder()
                .WithColor(color)
                .WithDescription(contents)
                .BuildToChannelAsync(channel);
        }
    }
}