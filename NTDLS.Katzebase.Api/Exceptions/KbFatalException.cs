﻿namespace NTDLS.Katzebase.Api.Exceptions
{
    public class KbFatalException : KbExceptionBase
    {
        public KbFatalException()
        {
            Severity = KbConstants.KbLogSeverity.Fatal;
        }

        public KbFatalException(string? message)
            : base(message)
        {
            Severity = KbConstants.KbLogSeverity.Fatal;
        }
    }
}
