﻿using NTDLS.Katzebase.Client.Payloads;
using NTDLS.Katzebase.Engine.Atomicity;
using NTDLS.Katzebase.Engine.Functions.Scaler;
using System.Text;
using static NTDLS.Katzebase.Client.KbConstants;
using NTDLS.Katzebase.Parsers.Interfaces;
using NTDLS.Katzebase.Parsers.Functions.System;
using NTDLS.Katzebase.Parsers.Functions.Scaler;
namespace NTDLS.Katzebase.Engine.Functions.System.Implementations
{
    public static class SystemShowScalerFunctions<TData> where TData : IStringable
    {
        public static KbQueryResultCollection<TData> Execute(EngineCore<TData> core, Transaction<TData> transaction, SystemFunctionParameterValueCollection<TData> function)
        {
            var collection = new KbQueryResultCollection<TData>();
            var result = collection.AddNew();

            result.AddField("Name");
            result.AddField("ReturnType");
            result.AddField("Parameters");
            result.AddField("Description");

            foreach (var prototype in ScalerFunctionCollection<TData>.Prototypes)
            {
                var parameters = new StringBuilder();

                foreach (var param in prototype.Parameters)
                {
                    parameters.Append($"{param.Type} {param.Name}");
                    if (param.HasDefault)
                    {
                        parameters.Append($" = {param.DefaultValue}");
                    }
                    parameters.Append(", ");
                }
                if (parameters.Length > 2)
                {
                    parameters.Length -= 2;
                }

                var values = new List<TData>(
                new[]{
                    prototype.Name,
                    prototype.ReturnType.ToString(),
                    parameters.ToString(),
                    prototype.Description
                }.Select(s => s.CastToT<TData>(EngineCore<TData>.StrCast)));
                result.AddRow(values);

#if DEBUG
                //This is to provide code for the documentation wiki.
                var wikiPrototype = new StringBuilder();

                wikiPrototype.Append($"##Color(#318000, {prototype.ReturnType})");
                wikiPrototype.Append($" ##Color(#c6680e, {prototype.Name})(");

                if (prototype.Parameters.Count > 0)
                {
                    for (int i = 0; i < prototype.Parameters.Count; i++)
                    {
                        var param = prototype.Parameters[i];

                        wikiPrototype.Append($"##Color(#318000, {param.Type}) ##Color(#c6680e, {param.Name})");
                        if (param.HasDefault)
                        {
                            wikiPrototype.Append($" = ##Color(#CC0000, \"'{param.DefaultValue}'\")");
                        }
                        wikiPrototype.Append(", ");
                    }
                    if (wikiPrototype.Length > 2)
                    {
                        wikiPrototype.Length -= 2;
                    }
                }
                wikiPrototype.Append(')');
                result.Messages.Add(new KbQueryResultMessage(wikiPrototype.ToString(), KbMessageType.Verbose));
#endif
            }

            return collection;
        }
    }
}
