﻿using ProtoBuf;

namespace NTDLS.Katzebase.PersistentTypes.Document
{
    /// <summary>
    /// This is the collection item that is physically written to disk via PhysicalDocumentPageCatalog.
    /// </summary>
    [ProtoContract]
    [Serializable]
    public class PhysicalDocumentPageCatalogItem
    {
        [ProtoMember(1)]
        public int PageNumber { get; set; }

        [ProtoMember(2)]
        public int DocumentCount { get; set; }

        public PhysicalDocumentPageCatalogItem(int pageNumber)
            => PageNumber = pageNumber;

        public PhysicalDocumentPageCatalogItem()
        {
        }
    }
}
