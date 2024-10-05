﻿using NTDLS.Katzebase.Engine.Schemas;
using NTDLS.Katzebase.Shared;

namespace NTDLS.Katzebase.PersistentTypes.Index
{
    [Serializable]
    public class PhysicalIndex
    {
        public List<PhysicalIndexAttribute> Attributes { get; set; } = new List<PhysicalIndexAttribute>();
        public string Name { get; set; } = string.Empty;
        public Guid Id { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        public uint Partitions { get; set; } = 1000;
        public bool IsUnique { get; set; } = false;
        public string GetPartitionPagesPath(PhysicalSchema physicalSchema)
            => Path.Combine(physicalSchema.DiskPath, $"@Index_{Shared.Helpers.MakeSafeFileName(Name)}");

        public string GetPartitionPagesFileName(PhysicalSchema physicalSchema, uint indexPartition)
            => Path.Combine(physicalSchema.DiskPath, $"@Index_{Shared.Helpers.MakeSafeFileName(Name)}", $"Page_{indexPartition}{EngineConstants.IndexPageExtension}");

        public PhysicalIndex()
        {
        }

        public uint ComputePartition(string? value)
        {
            uint hash = 0;
            if (string.IsNullOrEmpty(value))
            {
                return hash;
            }
            value = value.ToLowerInvariant();

            const uint seed = 131;
            foreach (char c in value)
            {
                hash = hash * seed + c;
            }
            return hash % Partitions;
        }

        public PhysicalIndex Clone()
        {
            var result = new PhysicalIndex
            {
                Id = Id,
                Name = Name,
                Created = Created,
                Modified = Modified,
                IsUnique = IsUnique,
                Partitions = Partitions,
            };

            foreach (var attribute in Attributes)
            {
                result.AddAttribute(attribute.Clone());
            }

            return result;
        }

        public void AddAttribute(string name)
        {
            AddAttribute(new PhysicalIndexAttribute()
            {
                Field = name
            });
        }

        public void AddAttribute(PhysicalIndexAttribute attribute)
            => AddAttribute(attribute);

        static public PhysicalIndex FromClientPayload(Client.Payloads.KbIndex index)
        {
            var persistIndex = new PhysicalIndex()
            {
                Id = index.Id,
                Name = index.Name,
                Created = index.Created,
                Modified = index.Modified,
                IsUnique = index.IsUnique,
                Partitions = index.Partitions
            };

            foreach (var attribute in index.Attributes)
            {
                persistIndex.AddAttribute(PhysicalIndexAttribute.FromClientPayload(attribute));
            }

            return persistIndex;
        }

        static public Client.Payloads.KbIndex ToClientPayload(PhysicalIndex index)
        {
            var persistIndex = new Client.Payloads.KbIndex()
            {
                Id = index.Id,
                Name = index.Name,
                Created = index.Created,
                Modified = index.Modified,
                IsUnique = index.IsUnique,
                Partitions = index.Partitions
            };

            foreach (var attribute in index.Attributes)
            {
                persistIndex.AddAttribute(PhysicalIndexAttribute.ToClientPayload(attribute));
            }

            return persistIndex;
        }
    }
}
