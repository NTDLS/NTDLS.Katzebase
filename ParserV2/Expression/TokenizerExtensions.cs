﻿using System.Text;

namespace ParserV2.Expression
{
    /// <summary>
    /// Used to walk various types of string and expressions.
    /// </summary>
    internal static class TokenizerExtensions
    {
        /// <summary>
        /// Splits the given text on a comma delimiter while paying attention to the scope denoted by open and close parentheses..
        /// </summary>
        public static List<string> ScopeSensitiveSplit(this string text)
            => ScopeSensitiveSplit(text, ',', '(', ')');

        /// <summary>
        /// Splits the given text on the delimiter while paying attention to the scope denoted by the given open and close characters.
        /// </summary>
        /// <returns></returns>
        public static List<string> ScopeSensitiveSplit(this string text, char splitOn, char open, char close)
        {
            int scope = 0;

            List<string> results = new();

            StringBuilder buffer = new();

            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == open)
                {
                    scope++;
                }
                else if (text[i] == close)
                {
                    scope--;
                }

                if (scope == 0 && text[i] == splitOn)
                {
                    results.Add(buffer.ToString().Trim());
                    buffer.Clear();
                }
                else
                {
                    buffer.Append(text[i]);
                }
            }

            if (buffer.Length > 0)
            {
                results.Add(buffer.ToString().Trim());
            }

            return results;
        }
    }
}
