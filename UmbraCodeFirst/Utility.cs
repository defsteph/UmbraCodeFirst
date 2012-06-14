using System;

namespace UmbraCodeFirst
{
    internal static class Utility
    {
        public static string FormatPropertyAlias(string alias)
        {
            if (String.IsNullOrWhiteSpace(alias) || alias.Length <= 1)
                return alias ?? String.Empty;

            return Char.ToLowerInvariant(alias[0]) + alias.Substring(1);
        }

    }
}