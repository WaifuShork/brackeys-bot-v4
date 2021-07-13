using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using BrackeysBot.Configuration;
using BrackeysBot.Database;
using BrackeysBot.Extensions;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.CommandsNext.Exceptions;
using DSharpPlus.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Serilog;
using Serilog.Events;

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
            
            try
            {
                commands.RegisterCommands(Assembly.GetExecutingAssembly());
                commands.RegisterCommandEvents();
                
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

        // I feel like this could be moved but does it really matter? the library is already handling all the heavy lifting
        private static async Task OnCommandErrorAsync(CommandsNextExtension commands, CommandErrorEventArgs args)
        {
            var failedChecks = ((ChecksFailedException) args.Exception).FailedChecks;
            if (failedChecks.OfType<RequireRolesAttribute>().Any())
            {
                await args.Context.Channel.SendColoredEmbedAsync("Access denied.", DiscordColor.Red);
            }

            if (failedChecks.OfType<RequireGuildAttribute>().Any())
            {
                await args.Context.Channel.SendColoredEmbedAsync("This command can only be executed in a guild", DiscordColor.Red);
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