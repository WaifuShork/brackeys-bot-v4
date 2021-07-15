using DSharpPlusNextGen;
using DSharpPlusNextGen.Entities;

namespace BrackeysBot.Extensions
{
    public static class DiscordMemberExtensions
    {
        public static string FormatName(this DiscordMember member)
        {
            return $"{member.Username}#{member.Discriminator}";
        }
        
        public static string EnsureAvatarUrl(this DiscordMember member)
        {
            return member.GetAvatarUrl(ImageFormat.Auto).WithAlternative(member.DefaultAvatarUrl);
        }
    }
}