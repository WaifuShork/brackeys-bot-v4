using System.Collections.Generic;
using System.ComponentModel;
using BrackeysBot.Attributes;
using Newtonsoft.Json;

namespace BrackeysBot.Configuration
{
    public class BotConfiguration
    {
        // Essentials
        [JsonProperty("token"), Confidential]
        [Description("The token that is used to log in the bot.")]
        public string Token { get; set; }

        [JsonProperty("prefix")]
        [Description("The prefix that the bot commands will use.")]
        public string Prefix { get; set; } = "[]";
        
        
        // Roles
        [JsonProperty("guildId")]
        [Description("The ID of the guild the bot is meant to act in.")]
        public ulong GuildID { get; set; }

        [JsonProperty("guruRoleId")]
        [Description("The ID of the role that identifies gurus.")]
        [ConfigDisplay(ConfigDisplayAttribute.Mode.RoleId)]
        public ulong GuruRoleID { get; set; }

        [JsonProperty("moderatorRoleId")]
        [Description("The ID of the role that identifies moderators.")]
        [ConfigDisplay(ConfigDisplayAttribute.Mode.RoleId)]
        public ulong ModeratorRoleID { get; set; }

        [JsonProperty("helperRoleId")]
        [Description("The ID of the role that identifies helpers.")]
        [ConfigDisplay(ConfigDisplayAttribute.Mode.RoleId)]
        public ulong HelperRoleID { get; set; }

        [JsonProperty("mutedRoleId")]
        [Description("The ID of the role that mutes someone.")]
        [ConfigDisplay(ConfigDisplayAttribute.Mode.RoleId)]
        public ulong MutedRoleID { get; set; }

        [JsonProperty("developerRoleId")]
        [Description("The ID of the bot developer role.")]
        [ConfigDisplay(ConfigDisplayAttribute.Mode.RoleId)]
        public ulong DeveloperRoleID { get; set; }
        
        [JsonProperty("teamRoleIds")]
        [Description("A list of team role IDs.")]
        [ConfigDisplay(ConfigDisplayAttribute.Mode.RoleId)]
        public ulong[] TeamRoleIDs { get; set; }

        [JsonProperty("userRoleIds")]
        [Description("A list of user role IDs.")]
        [ConfigDisplay(ConfigDisplayAttribute.Mode.RoleId)]
        public ulong[] UserRoleIDs { get; set; }
        
        [JsonProperty("loggableIds")]
        [Description("A list of IDs that should be logged when pinged.")]
        [ConfigDisplay(ConfigDisplayAttribute.Mode.RoleId)]
        public ulong[] LoggableIDs { get; set; }
        
        
        // Channels
        [JsonProperty("modLogChannelId")]
        [Description("The ID of the channel where moderation actions are logged.")]
        [ConfigDisplay(ConfigDisplayAttribute.Mode.ChannelId)]
        public ulong ModerationLogChannelID { get; set; }

        [JsonProperty("allowedCodeblockChannelIds")]
        [Description("A list of IDs where massive codeblocks are allowed.")]
        [ConfigDisplay(ConfigDisplayAttribute.Mode.ChannelId)]
        public ulong[] AllowedCodeblockChannelIDs { get; set; }

        [JsonProperty("amaChannelId")]
        [Description("The channel to extract ama questions from")]
        [ConfigDisplay(ConfigDisplayAttribute.Mode.ChannelId)]
        public ulong AmaChannelID { get; set; }

        [JsonProperty("moduleConfigs")]
        [Description("The configurations of the modules the bot uses.")]
        public Dictionary<string, bool> ModuleConfigurations { get; set; } = new();

        [JsonProperty("rules")]
        [Description("The rules that members in the servers should follow.")]
        [Shorten(50)]
        public string[] Rules { get; set; }

        [JsonProperty("codeblockThreshold")]
        [Description("The minimum length of a message to be seen as a codeblock.")]
        public int CodeblockThreshold { get; set; }

        [JsonProperty("blockedWords")]
        [Description("A list of regex for words which should automatically be deleted.")]
        public string[] BlockedWords { get; set; }
        
        [JsonProperty("clearMessageMaxHistory")]
        [Description("The maximum messages history count to fetch when clearing messages.")]
        public int ClearMessageMaxHistory { get; set; }

        [JsonProperty("infoCategoryId")]
        [Description("The ID of the category to display membercount.")]
        [ConfigDisplay(ConfigDisplayAttribute.Mode.ChannelId)]
        public ulong InfoCategoryId { get; set; }

        [JsonProperty("infoCategoryDisplay")]
        [Description("The display value of the info category. '%s%' will be replaced with membercount.")]
        public string InfoCategoryDisplay { get; set; }

        [JsonProperty("emoteRestrictions")]
        [Description("A list of channels and their emote restrictions")]
        public Dictionary<ulong, List<string>> EmoteRestrictions { get; set; }

        [JsonProperty("gamejamTimestamps")]
        [Description("The timestamps that outline a gamejam.")]
        public long[] GamejamTimestamps { get; set; }        
        
        [JsonProperty("endorseTimeoutMillis")]
        [Description("The minimum time between being able to endorse the same user again, in milliseconds")]
        public int EndorseTimeoutMillis { get; set; }
        
        [JsonProperty("latexTimeoutMillis")]
        [Description("The minimum time between being able to use the latex command again, in milliseconds")]
        public int LatexTimeoutMillis { get; set; }

        [JsonProperty("codeFormatterDeleteTresholdMillis")]
        [Description("The maximum time before a message can be deleted when the format code command is used by a guru, in milliseconds")]
        public int CodeFormatterDeleteTresholdMillis { get; set; }

        [JsonProperty("helperMuteMaxDuration")]
        [Description("The maximum amount of time a helper may mute a person")]
        public int HelperMuteMaxDuration { get; set; }

        [JsonProperty("muteUserIfUsingFilteredWord")]
        [Description("Whether or not to mute people using the filtered word")]
        public bool MuteUserIfUsingFilteredWord { get; set; }

        [JsonProperty("filteredWordMuteDuration")]
        [Description("The Duration to mute a person for when using a filtered word")]
        public int FilteredWordMuteDuration { get; set; }
    }
}