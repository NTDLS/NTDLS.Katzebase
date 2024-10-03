﻿using NTDLS.Katzebase.Client.Exceptions;
using NTDLS.Katzebase.Parsers.Query.Class.Helpers;
using NTDLS.Katzebase.Parsers.Query.SupportingTypes;
using NTDLS.Katzebase.Parsers.Query.WhereAndJoinConditions;
using NTDLS.Katzebase.Parsers.Tokens;
using NTDLS.Katzebase.Parsers.Interfaces;
namespace NTDLS.Katzebase.Parsers.Query.Class
{
    public static class StaticParserWhere<TData> where TData : IStringable
    {
        public static ConditionCollection<TData> Parse(QueryBatch<TData> queryBatch, Tokenizer<TData> tokenizer, Func<string, TData> parseStringToDoc, Func<string, TData> castStringToDoc
)
        {
            //Look for tokens that would mean the end of the where clause
            if (tokenizer.TryGetNextIndexOfAny([" group ", " order ", " offset ", " inner "], out int endOfWhere) == false)
            {
                //Maybe we end at the next query?
                if (tokenizer.TryEatCompareNext((o) => StaticParserUtility.IsStartOfQuery(o), out endOfWhere) == false)
                {
                    //Well, I suppose we will take the remainder of the query text.
                    endOfWhere = tokenizer.Length;
                }
            }

            string conditionText = tokenizer.EatSubStringAbsolute(endOfWhere).Trim();
            if (conditionText == string.Empty)
            {
                throw new KbParserException(tokenizer.GetCurrentLineNumber(), $"Found [{conditionText}], expected: list of conditions.");
            }

            return StaticConditionsParser.Parse(queryBatch, tokenizer, conditionText, parseStringToDoc, castStringToDoc
);
        }
    }
}
