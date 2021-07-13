using System;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Serilog;

namespace BrackeysBot.Configuration
{
    public class BotConfiguration
    {
        public static async Task<BotConfiguration> LoadConfigurationAsync(string configPath)
        {
            try
            {
                var contents = await File.ReadAllTextAsync(configPath);
                return JsonConvert.DeserializeObject<BotConfiguration>(contents);
            }
            catch (FileNotFoundException exception)
            {
                Log.Fatal(exception, $"Unable to locate file at '{configPath}', terminating host");
                return null;
            }
            catch (Exception exception)
            {
                Log.Fatal(exception, "Unable to deserialize configuration file, terminating host");
                return null;
            }
        }
        
        [JsonProperty("token")]
        [Description("The token that is used to log in the bot.")]
        public string Token { get; set; }

        [JsonProperty("prefix")]
        [Description("The prefix that the bot commands will use.")]
        public string Prefix { get; set; } = "[]";
        
        [JsonProperty("guildID")]
        [Description("The prefix that the bot commands will use.")]
        public ulong GuildID { get; set; }
    }
}