using System;
using DSharpPlus.Entities;

namespace BrackeysBot.Extensions
{
    public static class DiscordMessageExtensions
    {
        public static bool HasMentionPrefix(this DiscordMessage message, DiscordUser currentUser, out string output)
        {
            var contentSpan = message.Content.AsSpan();
            if (contentSpan.Length > 17 && contentSpan[0] == '<' && contentSpan[1] == '@')
            {
                var closingBracketIndex = contentSpan.IndexOf('>');
                if (closingBracketIndex != -1)
                {
                    var idSpan = contentSpan[2] == '!'
                        ? contentSpan.Slice(3, closingBracketIndex - 3)
                        : contentSpan.Slice(2, closingBracketIndex - 2);
                    
                    if (ulong.TryParse(idSpan, out var id) && id == currentUser.Id)
                    {
                        output = new string(contentSpan[(closingBracketIndex + 1)..]);
                        return true;
                    }
                }
            }

            output = string.Empty;
            return false;
        }
    }
}