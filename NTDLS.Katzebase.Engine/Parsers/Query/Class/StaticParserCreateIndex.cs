﻿using NTDLS.Katzebase.Client.Exceptions;
using NTDLS.Katzebase.Engine.Parsers.Query.Class.WithOptions;
using NTDLS.Katzebase.Engine.Parsers.Query.SupportingTypes;
using NTDLS.Katzebase.Engine.Parsers.Tokens;
using static NTDLS.Katzebase.Engine.Library.EngineConstants;

namespace NTDLS.Katzebase.Engine.Parsers.Query.Class
{
    internal static class StaticParserCreateIndex
    {
        internal static PreparedQuery Parse(QueryBatch<TData> queryBatch, Tokenizer tokenizer)
        {
            var query = new PreparedQuery(queryBatch, QueryType.Create)
            {
                SubQueryType = SubQueryType.Index
            };

            if (tokenizer.TryEatValidateNext((o) => TokenizerExtensions.IsIdentifier(o), out var indexName) == false)
            {
                throw new KbParserException($"Invalid query. Found [{indexName}], expected: index name.");
            }

            query.AddAttribute(PreparedQuery<TData>.QueryAttribute.IndexName, indexName);
            query.AddAttribute(PreparedQuery<TData>.QueryAttribute.IsUnique, false);

            tokenizer.IsNext('(');

            var indexFields = tokenizer.EatGetMatchingScope().Split(',').Select(o => o.Trim()).ToList();
            query.CreateIndexFields.AddRange(indexFields);

            tokenizer.EatIfNext("on");

            if (tokenizer.TryEatValidateNext((o) => TokenizerExtensions.IsIdentifier(o), out var schemaName) == false)
            {
                throw new KbParserException("Invalid query. Found '" + indexName + "', expected: schema name.");
            }
            query.Schemas.Add(new QuerySchema(schemaName));
            query.AddAttribute(PreparedQuery<TData>.QueryAttribute.Schema, schemaName);

            if (tokenizer.TryEatIfNext("with"))
            {
                var options = new ExpectedWithOptions
                {
                    {"partitions", typeof(uint) }
                };

                query.AddAttributes(StaticParserWithOptions.Parse(tokenizer, options));
            }

            return query;
        }
    }
}
