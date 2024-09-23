﻿using NTDLS.Katzebase.Engine.Atomicity;
using NTDLS.Katzebase.Engine.Indexes;
using NTDLS.Katzebase.Engine.Schemas;

namespace NTDLS.Katzebase.Engine.Threading.PoolingParameters
{
    /// <summary>
    /// Thread parameters for a index rebuild. Shared across all threads in a single operation.
    /// </summary>
    internal class RebuildIndexOperation
    {
        public Transaction Transaction { get; set; }
        public PhysicalSchema PhysicalSchema { get; set; }
        public PhysicalIndex PhysicalIndex { get; set; }
        public Dictionary<uint, PhysicalIndexPages> PhysicalIndexPageMap { get; set; }
        public object[] SyncObjects { get; private set; }

        public RebuildIndexOperation(Transaction transaction, PhysicalSchema physicalSchema,
            Dictionary<uint, PhysicalIndexPages> physicalIndexPageMap, PhysicalIndex physicalIndex, uint indexPartitions)
        {
            SyncObjects = new object[indexPartitions];

            for (uint indexPartition = 0; indexPartition < indexPartitions; indexPartition++)
            {
                SyncObjects[indexPartition] = new object();
            }

            Transaction = transaction;
            PhysicalSchema = physicalSchema;
            PhysicalIndex = physicalIndex;
            PhysicalIndexPageMap = physicalIndexPageMap;
        }

        /// <summary>
        /// Thread parameters for a index rebuild operation. Used by a single thread.
        /// </summary>
        internal class Instance(RebuildIndexOperation operation, int pageNumber)
        {
            public RebuildIndexOperation Operation { get; set; } = operation;
            public int PageNumber { get; set; } = pageNumber;
        }
    }
}
