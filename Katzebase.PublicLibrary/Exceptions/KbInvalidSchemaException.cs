﻿using static Katzebase.PublicLibrary.Constants;

namespace Katzebase.PublicLibrary.Exceptions
{
    public class KbInvalidSchemaException : KbExceptionBase
    {
        public KbInvalidSchemaException()
        {
            Severity = LogSeverity.Warning;
        }

        public KbInvalidSchemaException(string? message)
            : base($"Invalid schema exception: {message}.")

        {
            Severity = LogSeverity.Exception;
        }
    }
}