using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace BrackeysBot.Extensions
{
    public static class PrimitiveExtensions
    {
        public static string AsNullIfEmpty(this string s)
        {
            return string.IsNullOrEmpty(s) ? null : s;
        }

        public static string WithAlternative(this string s, string alternative)
        {
            return s.AsNullIfEmpty() ?? alternative;
        }
 
        public static string TrimTo(this string str, int maxLength, bool hideDots = false)
        {
            if (maxLength < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxLength), $"Argument {nameof(maxLength)} can't be negative.");
            }
            if (maxLength == 0)
            {
                return string.Empty;
            }
            if (maxLength <= 3)
            {
                return string.Concat(str.Select(_ => '.'));
            }
            if (str.Length < maxLength)
            {
                return str;
            }

            if (hideDots)
            {
                return string.Concat(str.Take(maxLength));
            }
            
            return string.Concat(str.Take(maxLength - 3)) + "...";
        }
    }
}