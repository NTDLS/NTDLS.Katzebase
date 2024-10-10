﻿using NTDLS.Helpers;
using NTDLS.Katzebase.Api.Exceptions;
using NTDLS.Katzebase.Parsers.Query.SupportingTypes;
using NTDLS.Katzebase.Parsers.Tokens;
using static NTDLS.Katzebase.Parsers.Constants;

namespace NTDLS.Katzebase.Parsers.Query.Specific
{
    public static class StaticParserAlterRole
    {
        internal static PreparedQuery Parse(QueryBatch queryBatch, Tokenizer tokenizer)
        {
            var query = new PreparedQuery(queryBatch, QueryType.Create, tokenizer.GetCurrentLineNumber())
            {
                SubQueryType = SubQueryType.Role
            };

            if (tokenizer.TryEatValidateNext((o) => TokenizerExtensions.IsIdentifier(o), out var roleName) == false)
            {
                throw new KbParserException(tokenizer.GetCurrentLineNumber(), $"Expected role name, found: [ {roleName} ].");
            }
            query.AddAttribute(PreparedQuery.Attribute.RoleName, roleName);

            tokenizer.EatIfNext(["add", "remove"], out var action);

            if (action.Is("add"))
            {
                query.SubQueryType = SubQueryType.AddUserToRole;

                if (tokenizer.TryEatValidateNext((o) => TokenizerExtensions.IsIdentifier(o), out var username) == false)
                {
                    throw new KbParserException(tokenizer.GetCurrentLineNumber(), $"Expected username, found: [ {username} ].");
                }
                query.AddAttribute(PreparedQuery.Attribute.Username, username);
            }
            else if (action.Is("remove"))
            {
                query.SubQueryType = SubQueryType.RemoveUserFromRole;

                if (tokenizer.TryEatValidateNext((o) => TokenizerExtensions.IsIdentifier(o), out var username) == false)
                {
                    throw new KbParserException(tokenizer.GetCurrentLineNumber(), $"Expected username, found: [ {username} ].");
                }
                query.AddAttribute(PreparedQuery.Attribute.Username, username);
            }
            else
            {
                throw new KbNotImplementedException("Unexpected token");
            }

            return query;
        }
    }
}
