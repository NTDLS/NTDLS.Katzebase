﻿namespace NTDLS.Katzebase.Parsers.Tokens
{
    public partial class Tokenizer
    {
        /// <summary>
        /// Gets the next token using the standard delimiters.
        /// </summary>
        public T GetNext<T>()
            => Helpers.Converters.ConvertTo<T>(GetNext(_standardTokenDelimiters));

        /// <summary>
        /// Gets the next token using the given delimiters.
        /// </summary>
        public T GetNext<T>(char[] delimiters)
            => Helpers.Converters.ConvertTo<T>(GetNext(delimiters));

        /// <summary>
        /// Returns the next token without moving the caret.
        /// </summary>
        public string GetNext()
            => GetNext(_standardTokenDelimiters);

        /// <summary>
        /// Returns the next token without moving the caret using the given delimiters.
        /// </summary>
        public string GetNext(char[] delimiters)
        {
            int restoreCaret = Caret;
            var token = EatGetNext(delimiters, out _);
            Caret = restoreCaret;
            return token;
        }

        /// <summary>
        /// Returns the next token without moving the caret using the given delimiters,
        ///     returns the delimiter character that the tokenizer stopped on through outStoppedOnDelimiter..
        /// </summary>
        public string GetNext(char[] delimiters, out char outStoppedOnDelimiter)
        {
            int restoreCaret = Caret;
            var token = EatGetNext(delimiters, out outStoppedOnDelimiter);
            Caret = restoreCaret;
            return token;
        }
    }
}
