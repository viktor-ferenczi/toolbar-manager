﻿namespace ToolbarManager.Extensions
{
    public static class StringExt
    {
        public static bool IsDigit(this char c) => c >= '0' && c <= '9';

        public static bool IsMatching(this string text, string[] pattern)
        {
            if (pattern == null)
                return true;

            foreach (var part in pattern)
            {
                if (!text.Contains(part))
                    return false;
            }

            return true;
        }
    }
}