﻿using System.Linq;
using System.Threading.Tasks;
using BrackeysBot.Extensions;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Humanizer;

namespace BrackeysBot.Modules
{
    public sealed partial class UsersModule
    {
        [Command("userinfo"), Aliases("whois")]
        [Description("Displays information about a specified user.")]
        public async Task DisplayUserInfoAsync(CommandContext ctx, DiscordMember member = null)
        {
            member ??= (DiscordMember) ctx.Message.Author;
            
            var userColor = DiscordColor.LightGray;

            var roles = member.Roles.ToList();
            if (roles.Count > 1)
            {
                userColor = member.Roles
                    .OrderByDescending(r => r.Position)
                    .Last().Color;
            }
            
            await new DiscordEmbedBuilder()
                .WithTitle($"Information about {member.Username}#{member.Discriminator}:")
                .WithThumbnail(member.EnsureAvatarUrl())
                .WithColor(userColor)
                .AddField("Username", member.ToString(), true)
                .AddField("ID", member.Id.ToString(), true)
                .AddFieldConditional(!string.IsNullOrEmpty(member.Nickname), "Nickname", member.Nickname, true)
                .AddField("Join Date", member.JoinedAt.DateTime.ToOrdinalWords(), true)
                .AddField("User Created", member.CreationTimestamp.DateTime.ToOrdinalWords())
                .BuildToChannelAsync(ctx.Channel);
        }
    }
}