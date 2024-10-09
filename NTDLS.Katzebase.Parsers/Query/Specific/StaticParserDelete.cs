﻿using NTDLS.Katzebase.Api.Exceptions;
using NTDLS.Katzebase.Parsers.Query.SupportingTypes;
using NTDLS.Katzebase.Parsers.Tokens;
using static NTDLS.Katzebase.Parsers.Constants;
using static NTDLS.Katzebase.Parsers.Query.SupportingTypes.QuerySchema;

namespace NTDLS.Katzebase.Parsers.Query.Specific
{
    public static class StaticParserDelete
    {
        internal static PreparedQuery Parse(QueryBatch queryBatch, Tokenizer tokenizer)
        {
            var query = new PreparedQuery(queryBatch, QueryType.Delete, tokenizer.GetCurrentLineNumber());

            if (tokenizer.TryEatIfNext("from"))
            {
                //Query: DELETE FROM [schema] WHERE [conditions]

                if (tokenizer.TryEatValidateNext((o) => TokenizerExtensions.IsIdentifier(o), out var schemaName) == false)
                {
                    throw new KbParserException(tokenizer.GetCurrentLineNumber(), $"Expected schema name, found: [{schemaName}].");
                }

                query.Schemas.Add(new QuerySchema(tokenizer.GetCurrentLineNumber(), schemaName.ToLowerInvariant(), QuerySchemaUsageType.Primary));
                query.AddAttribute(PreparedQuery.Attribute.TargetSchemaAlias, string.Empty);
            }
            else
            {
                //Query: DELETE [alias] FROM [schema] as [alias] INNER JOIN [schema] as [alias]

                if (tokenizer.TryEatValidateNext((o) => TokenizerExtensions.IsIdentifier(o), out var targetAlias) == false)
                {
                    throw new KbParserException(tokenizer.GetCurrentLineNumber(), $"Expected schema name, found: [{targetAlias}].");
                }
                query.AddAttribute(PreparedQuery.Attribute.TargetSchemaAlias, targetAlias.ToLowerInvariant());

                tokenizer.EatIfNext("from");

                if (tokenizer.TryEatValidateNext((o) => TokenizerExtensions.IsIdentifier(o), out var schemaName) == false)
                {
                    throw new KbParserException(tokenizer.GetCurrentLineNumber(), $"Expected schema name, found: [{schemaName}].");
                }

                if (tokenizer.TryEatIfNext("as"))
                {
                    var schemaAlias = tokenizer.EatGetNext();
                    query.Schemas.Add(new QuerySchema(tokenizer.GetCurrentLineNumber(), schemaName.ToLowerInvariant(), QuerySchemaUsageType.Primary, schemaAlias.ToLowerInvariant()));
                }
                else
                {
                    query.Schemas.Add(new QuerySchema(tokenizer.GetCurrentLineNumber(), schemaName.ToLowerInvariant(), QuerySchemaUsageType.Primary, schemaName.ToLowerInvariant()));
                }
            }

            //Parse joins.
            while (tokenizer.TryIsNext("inner"))
            {
                var joinedSchemas = StaticParserJoin.Parse(queryBatch, tokenizer);
                query.Schemas.AddRange(joinedSchemas);
            }

            if (tokenizer.TryEatIfNext("where"))
            {
                query.Conditions = StaticParserWhere.Parse(queryBatch, tokenizer);

                //Associate the root query schema with the root conditions.
                query.Schemas.First().Conditions = query.Conditions;
            }

            return query;
        }
    }
}