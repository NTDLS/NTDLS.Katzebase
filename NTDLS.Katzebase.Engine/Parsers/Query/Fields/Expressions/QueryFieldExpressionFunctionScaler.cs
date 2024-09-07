﻿using NTDLS.Katzebase.Engine.Parsers.Query.Functions;
using static NTDLS.Katzebase.Client.KbConstants;

namespace NTDLS.Katzebase.Engine.Parsers.Query.Fields.Expressions
{
    internal class QueryFieldExpressionFunctionScaler : IQueryFieldExpressionFunction
    {
        public string FunctionName { get; set; }
        public string ExpressionKey { get; set; }
        public KbBasicDataType ReturnType { get; set; }

        /// <summary>
        /// Parameter list for the this function.
        /// </summary>
        public List<IExpressionFunctionParameter> Parameters { get; set; } = new();

        public QueryFieldExpressionFunctionScaler(string functionName, string expressionKey, KbBasicDataType returnType)
        {
            FunctionName = functionName;
            ExpressionKey = expressionKey;
            ReturnType = returnType;
        }
    }
}
