﻿namespace NTDLS.Katzebase.Api.Exceptions
{
    public class KbFunctionException : KbExceptionBase
    {
        public KbFunctionException()
        {
            Severity = KbConstants.KbLogSeverity.Verbose;
        }

        public KbFunctionException(string message)
            : base(message)
        {
            Severity = KbConstants.KbLogSeverity.Verbose;
        }
    }
}
