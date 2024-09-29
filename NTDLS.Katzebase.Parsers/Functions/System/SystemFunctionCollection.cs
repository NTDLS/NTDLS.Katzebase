﻿using NTDLS.Helpers;
using NTDLS.Katzebase.Client.Exceptions;
using System.Diagnostics.CodeAnalysis;

namespace NTDLS.Katzebase.Parsers.Functions.System
{
    public static class SystemFunctionCollection
    {
        private static List<SystemFunction>? _protypes = null;
        public static List<SystemFunction> Prototypes
        {
            get
            {
                if (_protypes == null)
                {
                    throw new KbFatalException("Function prototypes were not initialized.");
                }
                return _protypes;
            }
        }

        public static void Initialize(string[] prototypeStrings)
        {
            if (_protypes == null)
            {
                _protypes = new();

                foreach (var prototype in prototypeStrings)
                {
                    _protypes.Add(SystemFunction.Parse(prototype));
                }
            }
        }

        public static bool TryGetFunction(string name, [NotNullWhen(true)] out SystemFunction? function)
        {
            function = Prototypes.FirstOrDefault(o => o.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
            return function != null;
        }

        public static SystemFunctionParameterValueCollection ApplyFunctionPrototype(string functionName, List<string?> parameters)
        {
            if (_protypes == null)
            {
                throw new KbFatalException("Function prototypes were not initialized.");
            }

            var function = _protypes.FirstOrDefault(o => o.Name.Is(functionName))
                ?? throw new KbFunctionException($"Undefined system function: [{functionName}].");

            return function.ApplyParameters(parameters);
        }
    }
}
