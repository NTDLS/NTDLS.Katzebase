﻿using NTDLS.Katzebase.Client.Exceptions;
using NTDLS.Katzebase.Parsers.Query.Class.WithOptions;
using NTDLS.Katzebase.Parsers.Query.SupportingTypes;
using NTDLS.Katzebase.Parsers.Tokens;
using static NTDLS.Katzebase.Parsers.Constants;
using NTDLS.Katzebase.Parsers.Interfaces;
namespace NTDLS.Katzebase.Parsers.Query.Class
{
    public static class StaticParserCreateIndex<TData> where TData : IStringable
    {
        internal static PreparedQuery<TData> Parse(QueryBatch<TData> queryBatch, Tokenizer<TData> tokenizer)
        {
            var query = new PreparedQuery<TData>(queryBatch, QueryType.Create)
            {
                SubQueryType = SubQueryType.Index
            };

            if (tokenizer.TryEatValidateNext((o) => TokenizerExtensions.IsIdentifier(o), out var indexName) == false)
            {
                throw new KbParserException(tokenizer.GetCurrentLineNumber(), $"Found [{indexName}], expected: index name.");
            }

            query.AddAttribute(PreparedQuery<TData>.QueryAttribute.IndexName, indexName);
            query.AddAttribute(PreparedQuery<TData>.QueryAttribute.IsUnique, false);

            tokenizer.IsNext('(');

            var indexFields = tokenizer.EatGetMatchingScope().Split(',').Select(o => o.Trim()).ToList();
            query.CreateIndexFields.AddRange(indexFields);

            tokenizer.EatIfNext("on");

            if (tokenizer.TryEatValidateNext((o) => TokenizerExtensions.IsIdentifier(o), out var schemaName) == false)
            {
                throw new KbParserException(tokenizer.GetCurrentLineNumber(), "Invalid query. Found '" + indexName + "', expected: schema name.");
            }
            query.Schemas.Add(new QuerySchema<TData>(schemaName));
            query.AddAttribute(PreparedQuery<TData>.QueryAttribute.Schema, schemaName);

            if (tokenizer.TryEatIfNext("with"))
            {
                var options = new ExpectedWithOptions<TData>
                {
                    {"partitions", typeof(uint) }
                };

                query.AddAttributes(StaticParserWithOptions.Parse<TData>(tokenizer, options));
            }

            return query;
        }
    }
}
