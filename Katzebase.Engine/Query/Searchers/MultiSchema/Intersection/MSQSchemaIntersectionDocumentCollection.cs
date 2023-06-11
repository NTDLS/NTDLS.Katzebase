﻿namespace Katzebase.Engine.Query.Searchers.MultiSchema.Intersection
{
    public class MSQSchemaIntersectionDocumentCollection
    {
        public List<MSQSchemaIntersectionDocumentItem> Documents { get; set; } = new();
        public int DistinctSchemaCount => Documents.Select(o => o.SchemaAlias).Distinct().Count();
    }
}
