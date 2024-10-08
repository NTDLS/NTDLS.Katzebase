﻿namespace NTDLS.Katzebase.Api.Exceptions
{
    public class KbDuplicateKeyViolationException : KbExceptionBase
    {
        public KbDuplicateKeyViolationException()
        {
            Severity = KbConstants.KbLogSeverity.Verbose;
        }

        public KbDuplicateKeyViolationException(string message)
            : base(message)
        {
            Severity = KbConstants.KbLogSeverity.Verbose;
        }
    }
}
