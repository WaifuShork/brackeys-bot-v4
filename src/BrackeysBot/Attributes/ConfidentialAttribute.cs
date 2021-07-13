using System;

namespace BrackeysBot.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public sealed class ConfidentialAttribute : Attribute
    {
        
    }
}