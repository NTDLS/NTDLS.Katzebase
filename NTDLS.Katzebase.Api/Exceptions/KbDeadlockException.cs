﻿namespace NTDLS.Katzebase.Api.Exceptions
{
    public class KbDeadlockException : KbExceptionBase
    {
        public KbDeadlockException()
        {
        }

        public KbDeadlockException(string message)
            : base(message)
        {
            Severity = KbConstants.KbLogSeverity.Verbose;
        }

        public KbDeadlockException(string message, string explanation)
            : base($"Deadlock: {message}\r\n\r\nExplanation:\r\n{explanation}")
        {
            Severity = KbConstants.KbLogSeverity.Verbose;
        }
    }
}
