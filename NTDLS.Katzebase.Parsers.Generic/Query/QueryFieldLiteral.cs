﻿using static NTDLS.Katzebase.Client.KbConstants;
using NTDLS.Katzebase.Parsers.Interfaces;
namespace NTDLS.Katzebase.Parsers.Query
{
    public class ConditionFieldLiteral<TData> where TData : IStringable
    {
        public TData? Value { get; set; }
        public KbBasicDataType DataType { get; set; }

        public ConditionFieldLiteral(KbBasicDataType dataType, TData? value)
        {
            DataType = dataType;
            Value = value;
        }
    }
}
