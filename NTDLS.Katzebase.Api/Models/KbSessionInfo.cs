﻿namespace NTDLS.Katzebase.Api.Models
{
    public class KbSessionInfo
    {
        public DateTime? ServerTimeUTC { get; set; }
        public Guid ConnectionId { get; set; }
        public ulong ProcessId { get; set; }
    }
}
