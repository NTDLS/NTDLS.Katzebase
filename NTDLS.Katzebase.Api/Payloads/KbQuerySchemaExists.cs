﻿using NTDLS.Katzebase.Api.Payloads.Response;
using NTDLS.ReliableMessaging;

namespace NTDLS.Katzebase.Api.Payloads
{
    public class KbQuerySchemaExists : IRmQuery<KbQuerySchemaExistsReply>
    {
        public Guid ConnectionId { get; set; }
        public string Schema { get; set; }

        public KbQuerySchemaExists(Guid connectionId, string schema)
        {
            ConnectionId = connectionId;
            Schema = schema;
        }
    }

    public class KbQuerySchemaExistsReply : KbActionResponseBoolean, IRmQueryReply
    {
        public KbQuerySchemaExistsReply()
        {

        }
        public KbQuerySchemaExistsReply(bool value) : base(value)
        {
        }
    }
}
