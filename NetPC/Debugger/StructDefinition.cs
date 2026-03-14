using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace NetPC.Debugger
{
    public class StructDefinition
    {
        public StructDefinition()
        {
            Fields = new List<StructField>();
        }
        public string Name { get; set; }
        public List<StructField> Fields { get; set; }
        public int BaseAddress { get; set; }

        [JsonIgnore]
        public int Length => Fields.Sum(f => f.FieldLen);
    }

    public class StructField
    {
        public int Order { get; set; }
        public string FieldName { get; set; } = "";
        public FieldType FieldType { get; set; }
        public int FieldLen { get; set; }

        internal object? DecodeVal(byte[] fValB)
        {
            if (FieldType == FieldType.fByte) return fValB[0];
            else if (FieldType == FieldType.fShort) return BitConverter.ToInt16(fValB);
            else if (FieldType == FieldType.fInt) return BitConverter.ToInt32(fValB);
            else if (FieldType == FieldType.fLong) return BitConverter.ToInt64(fValB);
            else if (FieldType == FieldType.fFloat) return BitConverter.ToHalf(fValB);
            else if (FieldType == FieldType.fBool) return BitConverter.ToBoolean(fValB);
            else if (FieldType == FieldType.fChar) return BitConverter.ToChar(fValB);
            else if (FieldType == FieldType.fString) return Encoding.UTF8.GetString(fValB);
            else return null;
        }
    }

    public enum FieldType
    {
        fByte = 0,
        fShort = 1,
        fInt = 2,
        fLong = 3,
        fFloat = 4,
        fDouble = 5,
        fBool = 6,
        fString = 7,
        fChar = 8,
        
    }
}
