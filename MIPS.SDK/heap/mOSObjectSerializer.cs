 
using System.Reflection;
using System.Text; 

namespace mOSLib.heap
{
    internal class mOSObjectSerializer
    {
        private static IEnumerable<PropertyInfo> GetOrderedMembers(object o)
        {
            return o.GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Select(m => (member: m, attr: m.GetCustomAttribute<FieldOrderAttribute>()))
                .Where(x => x.attr != null)
                .Select(x => new Tuple<int, PropertyInfo>(x.attr.Order, x.member))
                .OrderBy(x => x.Item1)
                .Select(x => x.Item2);
        }


        public void Desserialize(object o, byte[] data)
        {

            using (MemoryStream ms = new MemoryStream(data))
            using (BinaryReader br = new BinaryReader(ms))
            {
                foreach (PropertyInfo prop in GetOrderedMembers(o))
                {
                    Type propT = prop.PropertyType;

                    if (propT.IsArray || propT == typeof(string))
                    {
                        object value = DeserializeArray(prop, br);
                        prop.SetValue(o, value);
                    }
                    else
                    {
                        object value = DeserializeOne(prop, br);
                        prop.SetValue(o, value);
                    }
                }
            }
        }

        private static object DeserializeOne(PropertyInfo prop, BinaryReader br)
        {
            Type t = prop.PropertyType;

            if (!t.IsValueType)
                throw new InvalidOperationException($"Reference types are not allowed: {t}");

            if (Nullable.GetUnderlyingType(t) != null)
                throw new InvalidOperationException($"Nullable types are not allowed: {t}");

            if (t == typeof(byte)) return br.ReadByte();
            if (t == typeof(bool)) return br.ReadByte() != 0;
            if (t == typeof(short)) return br.ReadInt16();
            if (t == typeof(ushort)) return br.ReadUInt16();
            if (t == typeof(char)) return br.ReadChar();
            if (t == typeof(int)) return br.ReadInt32();
            if (t == typeof(uint)) return br.ReadUInt32();
            if (t == typeof(long)) return br.ReadInt64();
            if (t == typeof(ulong)) return br.ReadUInt64();
            if (t == typeof(double)) return br.ReadDouble();

            if (t == typeof(decimal))
            {
                int[] bits = new int[4];
                for (int i = 0; i < 4; i++)
                    bits[i] = br.ReadInt32();

                return new decimal(bits);
            }

            throw new NotSupportedException($"Type {t} is not supported by DeserializeOne");
        }

        private static object DeserializeArray(PropertyInfo prop, BinaryReader br)
        {
            Type t = prop.PropertyType;

            // 🔹 CASO ESPECIAL: string[]
            if (prop.PropertyType == typeof(string))
            {
                byte[] strSize = br.ReadBytes(4);
                if (strSize.Length == 0) return "";
                int size = BitConverter.ToInt32(strSize);
                byte[] strBin = br.ReadBytes(size);
                br.ReadBytes(1);

                string str = Encoding.UTF8.GetString(strBin);

                return str;
            }

            Type elemType = t.GetElementType();

            int totalBytes = br.ReadInt32();
            byte[] raw = br.ReadBytes(totalBytes);

            // 🔹 CASO NORMAL: tipos fixos
            int elemSize = GetFixedTypeSize(elemType);
            int count = totalBytes / elemSize;

            Array arr = Array.CreateInstance(elemType, count);

            int offset = 0;
            for (int i = 0; i < count; i++)
            {
                object val;

                if (elemType == typeof(byte)) val = raw[offset];
                else if (elemType == typeof(bool)) val = raw[offset] != 0;
                else if (elemType == typeof(short)) val = BitConverter.ToInt16(raw, offset);
                else if (elemType == typeof(ushort)) val = BitConverter.ToUInt16(raw, offset);
                else if (elemType == typeof(char)) val = BitConverter.ToChar(raw, offset);
                else if (elemType == typeof(int)) val = BitConverter.ToInt32(raw, offset);
                else if (elemType == typeof(uint)) val = BitConverter.ToUInt32(raw, offset);
                else if (elemType == typeof(long)) val = BitConverter.ToInt64(raw, offset);
                else if (elemType == typeof(ulong)) val = BitConverter.ToUInt64(raw, offset);
                else if (elemType == typeof(double)) val = BitConverter.ToDouble(raw, offset);
                else
                    throw new NotSupportedException($"Array element type {elemType} not supported");

                arr.SetValue(val, i);
                offset += elemSize;
            }

            return arr;
        }



        public byte[] Serialize(object o)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                foreach (PropertyInfo prop in GetOrderedMembers(o))
                {
                    Type propT = prop.PropertyType;
                    if (propT.IsArray || propT == typeof(string))
                    {
                        SerializeArray(o, prop, ms);
                    }
                    else
                        SerializeOne(o, prop, ms);
                }

                ms.Position = 0;
                return ms.ToArray();
            }
        }
        private static int GetFixedTypeSize(Type t)
        {
            if (t == typeof(byte)) return 1;
            if (t == typeof(bool)) return 1;
            if (t == typeof(short)) return 2;
            if (t == typeof(ushort)) return 2;
            if (t == typeof(char)) return 2;
            if (t == typeof(int)) return 4;
            if (t == typeof(uint)) return 4;
            if (t == typeof(float)) return 4;
            if (t == typeof(long)) return 8;
            if (t == typeof(ulong)) return 8;
            if (t == typeof(double)) return 8;

            throw new NotSupportedException($"Type {t} is not a fixed-size type");
        }


        private static void SerializeArray(object o, PropertyInfo prop, MemoryStream ms)
        {
            Type t = prop.PropertyType;




            // 🔹 CASO ESPECIAL: string[]
            if (t == typeof(string))
            {
                using (MemoryStream tmp = new MemoryStream())
                {
                    string val = $"{prop.GetValue(o)}";
                    byte[] raw = Encoding.UTF8.GetBytes(val);

                    ms.Write(BitConverter.GetBytes(raw.Length));
                    ms.Write(raw);
                    ms.Write(new byte[1] { 0 });
                }

                return;
            }


            Type elemType = t.GetElementType();
            Array arr = (Array)prop.GetValue(o);

            // 🔹 CASO NORMAL: tipos de tamanho fixo
            int elemSize = GetFixedTypeSize(elemType);
            int totalBytes = arr.Length * elemSize;

            // 1️⃣ grava o tamanho bruto (em bytes)
            ms.Write(BitConverter.GetBytes(totalBytes));

            // 2️⃣ pré-materializa o array
            byte[] rawFixed = new byte[totalBytes];
            int offset = 0;

            for (int i = 0; i < arr.Length; i++)
            {
                object val = arr.GetValue(i);

                if (elemType == typeof(byte))
                {
                    rawFixed = (byte[])arr;
                    break;
                }
                else if (elemType == typeof(bool)) rawFixed[offset] = (byte)((bool)val ? 1 : 0);
                else if (elemType == typeof(short)) Array.Copy(BitConverter.GetBytes((short)val), 0, rawFixed, offset, 2);
                else if (elemType == typeof(ushort)) Array.Copy(BitConverter.GetBytes((ushort)val), 0, rawFixed, offset, 2);
                else if (elemType == typeof(char)) Array.Copy(BitConverter.GetBytes((char)val), 0, rawFixed, offset, 2);
                else if (elemType == typeof(int)) Array.Copy(BitConverter.GetBytes((int)val), 0, rawFixed, offset, 4);
                else if (elemType == typeof(uint)) Array.Copy(BitConverter.GetBytes((uint)val), 0, rawFixed, offset, 4);
                else if (elemType == typeof(long)) Array.Copy(BitConverter.GetBytes((long)val), 0, rawFixed, offset, 8);
                else if (elemType == typeof(ulong)) Array.Copy(BitConverter.GetBytes((ulong)val), 0, rawFixed, offset, 8);
                else
                    throw new NotSupportedException($"Array element type {elemType} not supported");

                offset += elemSize;
            }

            // 3️⃣ grava o buffer bruto
            ms.Write(rawFixed);
        }



        private static void SerializeOne(object o, PropertyInfo prop, MemoryStream ms)
        {
            Type t = prop.PropertyType;

            if (!t.IsValueType)
                throw new InvalidOperationException(
                    $"Reference types are not allowed: {t}");

            if (Nullable.GetUnderlyingType(t) != null)
                throw new InvalidOperationException(
                    $"Nullable types are not allowed: {t}");

            object value = prop.GetValue(o);

            if (t == typeof(byte)) ms.WriteByte((byte)value);
            else if (t == typeof(short)) ms.Write(BitConverter.GetBytes((short)value));
            else if (t == typeof(ushort)) ms.Write(BitConverter.GetBytes((ushort)value));
            else if (t == typeof(int)) ms.Write(BitConverter.GetBytes((int)value));
            else if (t == typeof(uint)) ms.Write(BitConverter.GetBytes((uint)value));
            else if (t == typeof(long)) ms.Write(BitConverter.GetBytes((long)value));
            else if (t == typeof(ulong)) ms.Write(BitConverter.GetBytes((ulong)value));
            else if (t == typeof(bool)) ms.WriteByte((byte)((bool)value ? 1 : 0));
            else if (t == typeof(char)) ms.Write(BitConverter.GetBytes((char)value));
            else if (t == typeof(double))
            {
                byte[] bytes = BitConverter.GetBytes((double)value);
                ms.Write(bytes, 0, bytes.Length);
            }
            else if (t == typeof(decimal))
            {
                int[] bits = decimal.GetBits((decimal)value);
                for (int i = 0; i < 4; i++)
                    ms.Write(BitConverter.GetBytes(bits[i]), 0, 4);
            }
            else throw new NotSupportedException($"Type {t} is not supported by SerializeOne");
        }
    }
}
