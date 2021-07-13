using DSharpPlus;
using DSharpPlus.Entities;

namespace BrackeysBot.Extensions
{
    public static class DiscordMemberExtensions
    {
        public static string EnsureAvatarUrl(this DiscordMember member)
        {
            return member.GetAvatarUrl(ImageFormat.Auto).WithAlternative(member.DefaultAvatarUrl);
        }
    }
}