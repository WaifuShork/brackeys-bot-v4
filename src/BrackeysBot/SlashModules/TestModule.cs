using System;
using System.Linq;
using System.Threading.Tasks;
using BrackeysBot.Extensions;
using DSharpPlusNextGen;
using DSharpPlusNextGen.Entities;
using DSharpPlusNextGen.SlashCommands;
using Humanizer;
using Serilog;

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
        
        [SlashCommand("whois", "A whois command using slash commands :D")]
        public async Task WhoisAsync(InteractionContext ctx, [Option("member", "the member to select")] string input)
        {
            DiscordMember member = null;
            var stringId = string.Join("", input.Where(char.IsDigit));
            if (!ulong.TryParse(stringId, out var valueId))
            {
                var members = await ctx.Guild.GetAllMembersAsync();
                member = members.FirstOrDefault(m
                    => string.Equals(m.Username, input, StringComparison.InvariantCultureIgnoreCase));
            }
            else
            {
                member = await ctx.Guild.GetMemberAsync(valueId);
            }
            
            var userColor = DiscordColor.LightGray;

            var roles = member.Roles.ToList();
            if (roles.Count > 1)
            {
                userColor = member.Roles
                    .OrderByDescending(r => r.Position)
                    .First().Color;
            }
            
            var embed = new DiscordEmbedBuilder()
                .WithTitle($"Information about {member.FormatName()}:")
                .WithThumbnail(member.EnsureAvatarUrl())
                .WithColor(userColor)
                .AddField("Username", member.ToString(), true)
                .AddField("ID", member.Id.ToString(), true)
                .AddFieldConditional(!string.IsNullOrEmpty(member.Nickname), "Nickname", member.Nickname, true)
                .AddField("Join Date", member.JoinedAt.DateTime.ToOrdinalWords(), true)
                .AddField("User Created", member.CreationTimestamp.DateTime.ToOrdinalWords());

            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, 
                new DiscordInteractionResponseBuilder()
                    .AddEmbed(embed.Build()));
        }
    }
}