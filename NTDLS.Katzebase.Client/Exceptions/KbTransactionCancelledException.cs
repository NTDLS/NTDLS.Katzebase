﻿namespace NTDLS.Katzebase.Client.Exceptions
{
    public class KbTransactionCancelledException : KbExceptionBase
    {
        public KbTransactionCancelledException()
        {
            Severity = KbConstants.KbLogSeverity.Verbose;
        }

        public KbTransactionCancelledException(string message)
            : base(message)
        {
            Severity = KbConstants.KbLogSeverity.Verbose;
        }
    }
}
