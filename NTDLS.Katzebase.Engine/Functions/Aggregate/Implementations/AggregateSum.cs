﻿using NTDLS.Katzebase.Engine.QueryProcessing.Searchers.Intersection;

namespace NTDLS.Katzebase.Engine.Functions.Aggregate.Implementations
{
    internal static class AggregateSum<TData> where TData : IStringable
    {
        public static string Execute(GroupAggregateFunctionParameter<TData> parameters)
        {
            return parameters.AggregationValues.Sum(o => o.ToT<double>()).ToString();
        }
    }
}
