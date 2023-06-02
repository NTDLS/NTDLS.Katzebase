﻿using Katzebase.Engine.Indexes;
using static Katzebase.Engine.Constants;

namespace Katzebase.Engine.Query.Condition
{
    public class FlatConditionGroup
    {
        public List<ConditionSingle> Conditions = new();

        public LogicalConnector LogicalConnector { get; set; }

        public Guid SubsetUID { get; set; }
        public string SubsetVariableName { get; set; } = string.Empty;

        /// <summary>
        /// If this condition is covered by an index, this is the index which we will use.
        /// </summary>
        public IndexSelection? IndexSelection { get; set; }

        public ConditionSubset ToSubset()
        {
            var subset = new ConditionSubset(LogicalConnector, SubsetUID);

            foreach (var condition in Conditions)
            {
                subset.Conditions.Add(condition);
            }

            return subset;
        }

        public FlatConditionGroup(ConditionSubset subset)
        {
            SubsetUID = subset.SubsetUID;
            LogicalConnector = subset.LogicalConnector;
            IndexSelection = subset.IndexSelection;
        }
    }
}
