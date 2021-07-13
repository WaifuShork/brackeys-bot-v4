using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using BrackeysBot.Database;
using BrackeysBot.Extensions;
using BrackeysBot.Configuration;

using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.CommandsNext;
using DSharpPlus.SlashCommands;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.CommandsNext.Exceptions;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.Interactivity.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

using Serilog;
using Serilog.Events;

namespace BrackeysBot
{
    public static class BrackeysBotHost
    {
        private static DiscordClient _client;
        
        private static CommandsNextExtension _commands;
        private static SlashCommandsExtension _slashCommands;
        private static InteractivityExtension _interactivity;
        
        private static BotConfiguration _configuration;
        private static IServiceProvider _provider;

        public static async Task<int> RunHostAsync()
        {
            try
            {
                await BuildHostAsync();
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
        
        // Pretty much everything needed is here, all services are available via CommandContext, so we should be good for everything
        private static async Task BuildHostAsync()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                // Overrides level require for anything in the Microsoft namespace to be logged
                .MinimumLevel.Override("Microsoft", LogEventLevel.Error)
                .WriteTo.Console()
                // Creates a new log file every 24 hours with the current timestamp
                .WriteTo.File(Path.Combine("logs", "logs-.txt"), LogEventLevel.Verbose, rollingInterval: RollingInterval.Day)
                .CreateLogger();

            _configuration = await BotConfiguration.LoadConfigurationAsync("_config.json");

            var clientConfig = new DiscordConfiguration
            {
                AutoReconnect = true,
                LargeThreshold = 250,
                Token = _configuration.Token,
                TokenType = TokenType.Bot,
                MinimumLogLevel = LogLevel.Debug,
                AlwaysCacheMembers = true,
                MessageCacheSize = 2048,
                // D#+ has it's own console logging formats so to keep things uniform I just used Serilog
                LoggerFactory = new LoggerFactory().AddSerilog(),
                // I still barely understand this so... yeah, this works for now
                Intents = DiscordIntents.AllUnprivileged
            };

            _client = new DiscordClient(clientConfig);
            _client.RegisterClientEvents();

            // Setup singletons and hosted services, maybe this could be moved to a method depending on how big it gets? 
            _provider = new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton(_configuration)
                .AddDbContext<BrackeysBotContext>()
                .BuildServiceProvider();
            
            // Commands \\
            _commands = _client.UseCommandsNext(new CommandsNextConfiguration
            {
                Services = _provider,
                StringPrefixes = new[] { _configuration.Prefix },
                CaseSensitive = false,
            });
            _commands.RegisterCommands(Assembly.GetExecutingAssembly());
            _commands.RegisterCommandEvents();
            
            // Slash Commands \\
            _slashCommands = _client.UseSlashCommands(new SlashCommandsConfiguration
            {
                Services = _provider
            });
            _slashCommands.RegisterSlashCommands(Assembly.GetExecutingAssembly());
            _slashCommands.RegisterSlashCommandEvents();
            
            // Interactivity \\
            _interactivity = _client.UseInteractivity(new InteractivityConfiguration
            {
                PollBehaviour = PollBehaviour.KeepEmojis,
                Timeout = TimeSpan.FromSeconds(60),
                ResponseBehavior = InteractionResponseBehavior.Respond,
                ResponseMessage = "Unable to process your request"
            });
            _interactivity.RegisterInteractivityEvents();
        }

        private static void RegisterClientEvents(this DiscordClient client)
        {
            client.Ready += (_, _) => Task.CompletedTask;

            client.SocketErrored += (_, _) => Task.CompletedTask;
        }

        private static void RegisterCommandEvents(this CommandsNextExtension commands)
        {
            commands.CommandErrored += async (sender, args) =>
            {
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
            };
        }

        private static void RegisterSlashCommandEvents(this SlashCommandsExtension commands)
        {
            commands.SlashCommandErrored += (sender, args) =>
            {
                switch (args.Exception)
                {
                    case SlashExecutionChecksFailedException checksFailedException:
                        break;
                }

                return Task.CompletedTask;
            };
        }

        private static void RegisterInteractivityEvents(this BaseExtension interactivity)
        {
            interactivity.Client.InteractionCreated += (_, _) => Task.CompletedTask;
        }
        
        private static void RegisterSlashCommands(this SlashCommandsExtension slashCommands, Assembly assembly)
        {
            var commands = assembly.GetExportedTypes().Where(t => t.BaseType == typeof(SlashCommandModule));

            foreach (var command in commands)
            {
                slashCommands.RegisterCommands(command, _configuration.GuildID);
            }
        }
    }
}