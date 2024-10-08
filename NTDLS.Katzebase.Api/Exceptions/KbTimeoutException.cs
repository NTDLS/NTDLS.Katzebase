﻿namespace NTDLS.Katzebase.Api.Exceptions
{
    public class KbTimeoutException : KbExceptionBase
    {
        public KbTimeoutException()
        {
            Severity = KbConstants.KbLogSeverity.Verbose;
        }

        public KbTimeoutException(string message)
            : base(message)
        {
            Severity = KbConstants.KbLogSeverity.Verbose;
        }
    }
}
