﻿using NTDLS.Helpers;
using NTDLS.Katzebase.Client.Exceptions;
using NTDLS.Katzebase.Engine.Functions.Scaler;
using NTDLS.Katzebase.Engine.Library;
using NTDLS.Katzebase.Engine.Parsers.Query.SupportingTypes;
using NTDLS.Katzebase.Engine.Parsers.Query.WhereAndJoinConditions;
using NTDLS.Katzebase.Engine.Parsers.Tokens;
using static NTDLS.Katzebase.Engine.Library.EngineConstants;

namespace NTDLS.Katzebase.Engine.Parsers.Query.Class
{
    internal static class StaticConditionsParser
    {
        /// <summary>
        /// Parse the conditions, we are going to build a full expression with all of the condition values replaced with tokens so we
        /// can build a mathematical expression, but we are also going to build sets of groups for which there will be one per OR expression,
        /// and all conditions in a ConditionGroup will be comprised solely of AND conditions. This way we can use the groups to match indexes
        /// before evaluating the whole expression on the limited set of documents we derived from the indexing operations.
        /// </summary>
        public static ConditionCollection Parse(QueryBatch queryBatch, Tokenizer parentTokenizer, string conditionsText, string leftHandAliasOfJoin = "")
        {
            var conditionCollection = new ConditionCollection(queryBatch, conditionsText, leftHandAliasOfJoin);

            conditionCollection.MathematicalExpression = conditionCollection.MathematicalExpression
                .Replace(" OR ", " || ", StringComparison.InvariantCultureIgnoreCase)
                .Replace(" AND ", " && ", StringComparison.InvariantCultureIgnoreCase);

            ParseRecursive(queryBatch, parentTokenizer, conditionCollection, conditionCollection, conditionsText, LogicalConnector.None);

            conditionCollection.MathematicalExpression = conditionCollection.MathematicalExpression.Replace("  ", " ").Trim();

            conditionCollection.Hash = Library.Helpers.GetSHA256Hash(conditionCollection.MathematicalExpression);

            return conditionCollection;
        }

        private static void ParseRecursive(QueryBatch queryBatch, Tokenizer parentTokenizer,
            ConditionCollection conditionCollection, List<ConditionSet> parentConditionGroup, string conditionsText, LogicalConnector givenLogicalConnector)
        {
            var tokenizer = new Tokenizer(conditionsText);

            var lastLogicalConnector = LogicalConnector.None;

            ConditionSet? conditionGroup = null;
            Condition? lastCondition = null;

            while (!tokenizer.IsExhausted())
            {
                if (tokenizer.TryIsNextCharacter('('))
                {
                    string subConditionsText = tokenizer.EatMatchingScope();
                    ParseRecursive(queryBatch, parentTokenizer, conditionCollection, lastCondition?.Children ?? parentConditionGroup, subConditionsText, lastLogicalConnector);

                    if (!tokenizer.IsExhausted())
                    {
                        if (tokenizer.EatIsNextEnumToken<LogicalConnector>() == LogicalConnector.Or)
                        {
                            conditionGroup = new ConditionSet(LogicalConnector.Or);
                            parentConditionGroup.Add(conditionGroup);
                        }
                    }

                    continue;
                }

                var leftAndRight = ParseRightAndLeft(conditionCollection, parentTokenizer, tokenizer);

                lastCondition = new Condition(leftAndRight.ExpressionVariable, leftAndRight.Left, leftAndRight.Qualifier, leftAndRight.Right);

                if (conditionGroup == null)
                {
                    //We late initialize here because the first thing we encounter might be a parenthesis
                    //instead of a condition and we don't want to have added an empty condition group.
                    conditionGroup = new ConditionSet(givenLogicalConnector);
                    parentConditionGroup.Add(conditionGroup);
                }

                conditionGroup.Add(lastCondition);

                if (!tokenizer.IsExhausted())
                {
                    lastLogicalConnector = tokenizer.EatIsNextEnumToken<LogicalConnector>();
                    if (lastLogicalConnector == LogicalConnector.Or)
                    {
                        conditionGroup = new ConditionSet(LogicalConnector.Or);
                        parentConditionGroup.Add(conditionGroup);
                    }
                }
            }
        }

        private static ConditionPair ParseRightAndLeft(ConditionCollection conditionCollection, Tokenizer parentTokenizer, Tokenizer tokenizer)
        {
            int startLeftRightCaret = tokenizer.Caret;
            int startConditionSetCaret = tokenizer.Caret;
            string? leftExpressionString = null;
            string? rightExpressionString = null;

            LogicalQualifier logicalQualifier = LogicalQualifier.None;

            //Here we are just validating the condition tokens and finding the end of the condition pair values as well as the logical qualifier.
            while (!tokenizer.IsExhausted())
            {
                string token = tokenizer.GetNext();

                if (StaticConditionHelpers.IsLogicalQualifier(token))
                {
                    //This is a logical qualifier, we're all good. We now have the left expression and the qualifier.
                    leftExpressionString = tokenizer.Substring(startLeftRightCaret, tokenizer.Caret - startLeftRightCaret).Trim();
                    logicalQualifier = StaticConditionHelpers.ParseLogicalQualifier(token);
                    tokenizer.EatNext();
                    startLeftRightCaret = tokenizer.Caret;
                }
                else if (StaticConditionHelpers.IsLogicalConnector(token))
                {
                    //This is a logical qualifier, we're all good. We now have the right expression.
                    break;
                }
                else if (token.Length == 1 && (token[0].IsTokenConnectorCharacter() || token[0].IsMathematicalOperator()))
                {
                    //This is a connector character, we're all good.
                    tokenizer.EatNext();
                }
                else if (token.StartsWith("$s_") && token.EndsWith('$')) //A string placeholder.
                {
                    //This is a string placeholder, we're all good.
                    tokenizer.EatNext();
                }
                else if (token.StartsWith("$n_") && token.EndsWith('$')) //A numeric placeholder.
                {
                    //This is a numeric placeholder, we're all good.
                    tokenizer.EatNext();
                }
                else if (token.StartsWith("$n_") && token.EndsWith('$')) //A numeric placeholder.
                {
                    //This is a numeric placeholder, we're all good.
                    tokenizer.EatNext();
                }
                else if (ScalerFunctionCollection.TryGetFunction(token, out var scalerFunction))
                {
                    if (!tokenizer.IsNextNonIdentifier(['(']))
                    {
                        throw new KbParserException($"Function [{token}] must be called with parentheses.");
                    }
                    //This is a scaler function, we're all good.

                    tokenizer.EatNext();
                    tokenizer.EatGetMatchingScope('(', ')');
                }
                else if (token.IsQueryFieldIdentifier())
                {
                    if (tokenizer.IsNextNonIdentifier(['(']))
                    {
                        //The character after this identifier is an open parenthesis, so this
                        //  looks like a function call but the function is undefined.
                        throw new KbParserException($"Function [{token}] is undefined.");
                    }

                    //This is a document field, we're all good.
                    tokenizer.EatNext();
                }
                else
                {
                    throw new NotImplementedException();
                }
            }

            rightExpressionString = tokenizer.Substring(startLeftRightCaret, tokenizer.Caret - startLeftRightCaret).Trim();

            var left = StaticParserField.Parse(parentTokenizer, leftExpressionString.EnsureNotNullOrEmpty(), conditionCollection.FieldCollection);
            var right = StaticParserField.Parse(parentTokenizer, rightExpressionString.EnsureNotNullOrEmpty(), conditionCollection.FieldCollection);

            var result = new ConditionPair(conditionCollection.NextExpressionVariable(), left, logicalQualifier, right);

            //Replace the condition with the name of the variable that must be evaluated to determine the value for this condition.
            string conditionSetText = tokenizer.Substring(startConditionSetCaret, tokenizer.Caret - startConditionSetCaret).Trim();
            conditionCollection.MathematicalExpression = conditionCollection.MathematicalExpression.ReplaceFirst(conditionSetText, result.ExpressionVariable);

            conditionCollection.FieldCollection.Add(new QueryField(string.Empty, conditionCollection.FieldCollection.Count, left));
            conditionCollection.FieldCollection.Add(new QueryField(string.Empty, conditionCollection.FieldCollection.Count, right));

            return result;
        }
    }
}
