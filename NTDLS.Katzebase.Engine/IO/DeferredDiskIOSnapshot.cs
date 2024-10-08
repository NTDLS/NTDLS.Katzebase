﻿using NTDLS.Katzebase.Api.Types;
using static NTDLS.Katzebase.Shared.EngineConstants;

namespace NTDLS.Katzebase.Engine.IO
{
    /// <summary>
    /// Snapshot class for DeferredDiskIO, used to snapshot the state of the associated class.
    /// </summary>
    internal class DeferredDiskIOSnapshot
    {
        /// <summary>
        /// Snapshot class for DeferredDiskIOObject, used to snapshot the state of the associated class.
        /// </summary>
        public class DeferredDiskIOObjectSnapshot(string diskPath, IOFormat format)
        {
            public string DiskPath { get; private set; } = diskPath.ToLowerInvariant();
            public IOFormat Format { get; private set; } = format;
        }

        public KbInsensitiveDictionary<DeferredDiskIOObjectSnapshot> Collection { get; } = new();

        public bool ContainsKey(string key) => Collection.ContainsKey(key);

        public DeferredDiskIOSnapshot()
        {
        }

        public int Count()
        {
            lock (this)
            {
                return Collection.Count;
            }
        }
    }
}
