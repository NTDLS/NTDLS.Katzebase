﻿using Newtonsoft.Json;

namespace Katzebase.Engine.Functions
{
    [Serializable]
    public class PhysicalProcedureCatalog
    {
        public List<PhysicalProcedure> Collection = new();

        [JsonIgnore]
        public string? DiskPath { get; set; }

        public void Remove(PhysicalProcedure item)
        {
            Collection.Remove(item);
        }

        public void Add(PhysicalProcedure item)
        {
            Collection.Add(item);
        }

        public PhysicalProcedure? GetById(Guid id)
        {
            return (from o in Collection where o.Id == id select o).FirstOrDefault();
        }

        public PhysicalProcedure? GetByName(string name)
        {
            foreach (var item in Collection)
            {
                if (string.Equals(item.Name, name, StringComparison.OrdinalIgnoreCase))
                {
                    return item;
                }
            }
            return null;
        }

        public PhysicalProcedureCatalog Clone()
        {
            var catalog = new PhysicalProcedureCatalog();

            lock (this)
            {
                foreach (var obj in Collection)
                {
                    catalog.Collection.Add(obj.Clone());
                }
            }

            return catalog;
        }
    }
}
