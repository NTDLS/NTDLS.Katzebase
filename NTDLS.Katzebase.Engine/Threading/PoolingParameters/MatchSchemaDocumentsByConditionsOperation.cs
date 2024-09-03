﻿using NTDLS.Katzebase.Client.Types;
using NTDLS.Katzebase.Engine.Atomicity;
using NTDLS.Katzebase.Engine.Documents;
using NTDLS.Katzebase.Engine.Indexes.Matching;
using NTDLS.Katzebase.Engine.Query.Constraints;
using NTDLS.Katzebase.Engine.Schemas;

namespace NTDLS.Katzebase.Engine.Threading.PoolingParameters
{
    /// <summary>
    /// Thread parameters for a lookup operations. Shared across all threads in a single operation.
    /// </summary>
    class MatchSchemaDocumentsByConditionsOperation
    {
        public Dictionary<uint, DocumentPointer> ThreadResults = new();
        public Transaction Transaction { get; set; }
        public IndexingConditionLookup Lookup { get; set; }
        public PhysicalSchema PhysicalSchema { get; set; }
        public string WorkingSchemaPrefix { get; set; }
        public Condition Condition { get; set; }

        public KbInsensitiveDictionary<string>? KeyValues { get; set; }

        public MatchSchemaDocumentsByConditionsOperation(Transaction transaction, IndexingConditionLookup lookup,
            PhysicalSchema physicalSchema, string workingSchemaPrefix, Condition condition, KbInsensitiveDictionary<string>? keyValues = null)
        {
            Transaction = transaction;
            Lookup = lookup;
            PhysicalSchema = physicalSchema;
            WorkingSchemaPrefix = workingSchemaPrefix;
            Condition = condition;
            KeyValues = keyValues;
        }

        /// <summary>
        /// Thread parameters for a lookup operations. Used by a single thread.
        /// </summary>
        public class Instance
        {
            public MatchSchemaDocumentsByConditionsOperation Operation { get; set; }
            public uint IndexPartition { get; set; }

            public Instance(MatchSchemaDocumentsByConditionsOperation operation, uint indexPartition)
            {
                Operation = operation;
                IndexPartition = indexPartition;
            }
        }
    }
}
