﻿using NTDLS.Katzebase.Client.Exceptions;
using NTDLS.Katzebase.Parsers.Query.SupportingTypes;
using NTDLS.Katzebase.Parsers.Tokens;
using static NTDLS.Katzebase.Parsers.Constants;
using NTDLS.Katzebase.Parsers.Interfaces;

namespace NTDLS.Katzebase.Parsers.Query.Class
{
    public static class StaticParserAlter<TData> where TData : IStringable
    {
        internal static PreparedQuery<TData> Parse(QueryBatch<TData> queryBatch, Tokenizer<TData> tokenizer)
        {
            var querySubType = tokenizer.EatIfNextEnum([SubQueryType.Schema, SubQueryType.Configuration]);

            return querySubType switch
            {
                SubQueryType.Schema => StaticParserAlterSchema<TData>.Parse(queryBatch, tokenizer),
                SubQueryType.Configuration => StaticParserAlterConfiguration<TData>.Parse(queryBatch, tokenizer),

                _ => throw new KbParserException(tokenizer.GetCurrentLineNumber(), $"The query type is not implemented: [{querySubType}].")
            };
        }
    }
}
