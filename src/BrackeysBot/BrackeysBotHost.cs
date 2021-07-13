using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using BrackeysBot.Database;
using BrackeysBot.Extensions;
using BrackeysBot.SlashModules;
using BrackeysBot.Configuration;

using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.CommandsNext;
using DSharpPlus.SlashCommands;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.CommandsNext.Exceptions;
using DSharpPlus.SlashCommands.EventArgs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

using Serilog;
using Serilog.Events;
using Newtonsoft.Json;

namespace BrackeysBot
{
    public static class BrackeysBotHost
    {
        private static DiscordClient _client;
        
        private static BotConfiguration _configuration;
        private static IServiceProvider _provider;

        // Pretty much everything needed is here, all services are available via CommandContext, so we should be good for everything
        public static async Task<int> RunAsync()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                // Overrides level require for anything in the Microsoft namespace to be logged
                .MinimumLevel.Override("Microsoft", LogEventLevel.Error)
                .WriteTo.Console()
                // Creates a new log file every 24 hours with the current timestamp
                .WriteTo.File(Path.Combine("logs", "logs-.txt"), LogEventLevel.Verbose, rollingInterval: RollingInterval.Day)
                .CreateLogger();

            _configuration = await LoadConfigurationAsync("_config.json");
            
            _client = new DiscordClient(new DiscordConfiguration()
            {
                Token = _configuration.Token,
                TokenType = TokenType.Bot,
                MinimumLogLevel = LogLevel.Debug,
                // D#+ has it's own console logging formats so to keep things uniform I just used Serilog
                LoggerFactory = new LoggerFactory().AddSerilog(),
                // I still barely understand this so... yeah, this works for now
                Intents = DiscordIntents.AllUnprivileged
            });

            // Setup singletons and hosted services, maybe this could be moved to a method depending on how big it gets? 
            _provider = new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton(_configuration)
                .AddDbContext<BrackeysBotContext>()
                .BuildServiceProvider();
            
            var commands = _client.UseCommandsNext(new CommandsNextConfiguration
            {
                Services = _provider,
                // I doubt we want extra prefixes but the option is there
                StringPrefixes = new[] { _configuration.Prefix },
                CaseSensitive = false,
            });

            var slashCommands = _client.UseSlashCommands();

            try
            {
                commands.RegisterCommands(Assembly.GetExecutingAssembly());
                commands.RegisterCommandEvents();
                
                slashCommands.RegisterSlashCommands(Assembly.GetExecutingAssembly());
                slashCommands.RegisterSlashCommandEvents();
                
                await _client.ConnectAsync();
                await Task.Delay(-1);
                return 0;
            }
            catch (Exception exception)
            {
                Log.Fatal(exception, "Host terminated unexpectedly");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        // tbh this doesn't need to be a method, but I think event registration should be out of the mess
        private static void RegisterCommandEvents(this CommandsNextExtension commands)
        {
            commands.CommandErrored += async (sender, args) => await OnCommandErrorAsync(sender, args);
        }

        private static void RegisterSlashCommandEvents(this SlashCommandsExtension slashCommands)
        {
            slashCommands.SlashCommandErrored += async (sender, args) => await OnSlashCommandErrorAsync(sender, args);
        }
        
        private static void RegisterSlashCommands(this SlashCommandsExtension slashCommands, Assembly assembly)
        {
            var commands = assembly.GetExportedTypes().Where(t => t.BaseType == typeof(SlashCommandModule));

            foreach (var command in commands)
            {
                slashCommands.RegisterCommands(command, _configuration.GuildID);
            }
        }

        private static Task OnSlashCommandErrorAsync(SlashCommandsExtension sender, SlashCommandErrorEventArgs args)
        {
            switch (args.Exception)
            {
                case SlashExecutionChecksFailedException checksFailedException:
                    break;
            }

            return Task.CompletedTask;
        }


        // I feel like this could be moved but does it really matter? the library is already handling all the heavy lifting
        private static async Task OnCommandErrorAsync(CommandsNextExtension commands, CommandErrorEventArgs args)
        {
            // Basically equivalent to IResult in Discord.NET, but with exceptions... 
            switch (args.Exception)
            {
                case ChecksFailedException checksFailedException:
                    var failedChecks = checksFailedException.FailedChecks;
                    // Unlike V3, we can add [RequireGuild] on every module, then selectively enable commands to be allowed in DMs,
                    // this way nothing will slip through and accidentally be allowed in DMs that isn't meant to be
                    if (failedChecks.OfType<RequireGuildAttribute>().Any())
                    {
                        await args.Context.Channel.SendColoredEmbedAsync("This command can only be executed in a guild", DiscordColor.Red);
                    }
                    break;
                case CommandNotFoundException commandNotFoundException:
                    break;
                case DuplicateCommandException duplicateCommandException:
                    break;
                case DuplicateOverloadException duplicateOverloadException:
                    break;
                case InvalidOverloadException invalidOverloadException:
                    break;
            }
        }

        // Since the configuration is loaded into memory on startup, there's no need creating an entire service dedicated,
        // to loading and reading the file -- just add Configuration to the service container and then it's always available :D 
        private static async Task<BotConfiguration> LoadConfigurationAsync(string configPath)
        {
            try
            {
                var contents = await File.ReadAllTextAsync(configPath);
                return JsonConvert.DeserializeObject<BotConfiguration>(contents);
            }
            catch (Exception exception)
            {
                Log.Fatal(exception, "Unable to load configuration file");
                return null;
            }
        }
    }
}