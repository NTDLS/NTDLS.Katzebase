﻿using NTDLS.Katzebase.Api.Exceptions;
using NTDLS.Katzebase.Parsers.Query.SupportingTypes;
using NTDLS.Katzebase.Parsers.Tokens;
using static NTDLS.Katzebase.Parsers.Constants;

namespace NTDLS.Katzebase.Parsers.Query.Specific
{
    public static class StaticParserKill
    {
        internal static PreparedQuery Parse(QueryBatch queryBatch, Tokenizer tokenizer)
        {
            var query = new PreparedQuery(queryBatch, QueryType.Kill, tokenizer.GetCurrentLineNumber());

            var referencedProcessId = tokenizer.EatGetNextEvaluated<ulong>();
            try
            {
                query.AddAttribute(PreparedQuery.Attribute.ProcessId, referencedProcessId);
            }
            catch
            {
                throw new KbParserException(tokenizer.GetCurrentLineNumber(), $"Expected numeric process id, found: [{referencedProcessId}].");
            }

            return query;
        }
    }
}
