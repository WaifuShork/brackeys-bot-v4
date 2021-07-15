using System;
using System.Threading.Tasks;
using DSharpPlusNextGen;
using DSharpPlusNextGen.Common.Utilities;
using DSharpPlusNextGen.Entities;
using DSharpPlusNextGen.Enums;
using DSharpPlusNextGen.EventArgs;

namespace BrackeysBot
{
    public class EventDrivenDiscordButtonComponent
    {
        private readonly DiscordButtonComponent _button;

        public EventDrivenDiscordButtonComponent(string label, DiscordClient client)
            : this(new DiscordButtonComponent(ButtonStyle.Secondary, $"btn-{Guid.NewGuid()}", label), client)
        {
        }
    
        public EventDrivenDiscordButtonComponent(ButtonStyle style, string label, DiscordClient client)
            : this(new DiscordButtonComponent(style, $"btn-{Guid.NewGuid()}", label), client)
        {
        }
    
        public EventDrivenDiscordButtonComponent(ButtonStyle style, string customId, string label, DiscordClient client)
            : this(new DiscordButtonComponent(style, customId, label), client)
        {
        }
    
        public EventDrivenDiscordButtonComponent(DiscordButtonComponent button, DiscordClient client)
        {
            _button = button;
        
            client.ComponentInteractionCreated += ClientOnComponentInteractionCreated;
        }

        public event AsyncEventHandler<DiscordClient, ComponentInteractionCreateEventArgs> Clicked = null!;

        public static implicit operator DiscordButtonComponent(EventDrivenDiscordButtonComponent component) => component._button;

        private async Task ClientOnComponentInteractionCreated(DiscordClient sender, ComponentInteractionCreateEventArgs e)
        {
            if (e.Id != _button.CustomId)
            {
                return;
            }

            await e.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage);
            await Task.Run(() => Clicked?.Invoke(sender, e));
        }
    }
}