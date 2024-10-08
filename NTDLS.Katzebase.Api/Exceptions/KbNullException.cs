﻿namespace NTDLS.Katzebase.Api.Exceptions
{
    public class KbNullException : KbExceptionBase
    {
        public KbNullException()
        {
            Severity = KbConstants.KbLogSeverity.Verbose;
        }

        public KbNullException(string message)
            : base(message)
        {
            Severity = KbConstants.KbLogSeverity.Verbose;
        }
    }
}
