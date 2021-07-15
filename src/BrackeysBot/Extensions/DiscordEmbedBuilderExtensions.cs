using System.Threading.Tasks;
using DSharpPlusNextGen.Entities;

namespace BrackeysBot.Extensions
{
    public static class DiscordEmbedBuilderExtensions
    {
        // MaxFieldValueLength = 1024
        // MaxFieldNameLength = 256
        
        // MaxDescriptionLength = 2048
        // MaxEmbedLength = 6000
        // MaxFieldCount = 25
        // MaxTitleLength = 256
        
        public static async Task<DiscordMessage> BuildToChannelAsync(this DiscordEmbedBuilder builder, DiscordChannel channel)
        {
            return await channel.SendMessageAsync(builder.Build());
        }
        
        public static DiscordEmbedBuilder AddFieldConditional<T>(this DiscordEmbedBuilder eb, bool condition, string name, T value, bool inline = false)
        {
            if (condition) 
            {
                var toPost = value?.ToString();

                eb.AddField(name, toPost.TrimTo(1024), inline);
            }

            return eb;
        }
    }
}