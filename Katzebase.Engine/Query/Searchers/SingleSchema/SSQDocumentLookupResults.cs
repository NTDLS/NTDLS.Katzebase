﻿namespace Katzebase.Engine.Query.Searchers.SingleSchema
{
    public class SSQDocumentLookupResults
    {
        public List<SSQDocumentLookupResult> Collection { get; set; } = new();

        public void Add(SSQDocumentLookupResult result)
        {
            Collection.Add(result);
        }

        public void AddRange(List<SSQDocumentLookupResult> result)
        {
            Collection.AddRange(result);
        }

        public void AddRange(SSQDocumentLookupResults result)
        {
            Collection.AddRange(result.Collection);
        }
    }
}
