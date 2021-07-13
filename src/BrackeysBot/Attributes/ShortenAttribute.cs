using System;

namespace BrackeysBot.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class ShortenAttribute : Attribute
    {
        public ShortenAttribute(int length)
        {
            Length = length;
        }
        
        public int Length { get; }
    }
}