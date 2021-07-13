using System;

namespace BrackeysBot.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class ConfigDisplayAttribute : Attribute
    {
        public enum Mode
        {
            UserId,
            ChannelId,
            RoleId
        }

        public ConfigDisplayAttribute(string format)
        {
            Format = format;
        }

        public ConfigDisplayAttribute(Mode mode)
        {
            Format = mode switch
            {
                Mode.UserId => "<@{0}>",
                Mode.ChannelId => "<#{0}>",
                Mode.RoleId => "<@&{0}>",
                _ => throw new AggregateException(nameof(mode))
            };
        }
        
        public string Format { get; }

        public string FormatValue<T>(params T[] args)
        {
            return string.Format(Format, args);
        }
    }
}