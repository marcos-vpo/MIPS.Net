
namespace MIPS.Net.SoC
{
    internal class CacheL1
    {
        public int[] addresses = new int[64];
        public byte[] values = new byte[64 * 4];

        int current_index = 0;
        public CacheL1()
        {
            current_index = addresses.Length - 1;
        }

        public void Clear()
        {
            for (int i = 0; i < addresses.Length; i++)
            {
                addresses[i] = 0;
                values[i] = 0;
            }
            current_index = addresses.Length - 1;
        }

        public void StoreInstruction(int address, byte[] value)
        {
            if (value == null) return;

            addresses[current_index] = address;

            int valueAddr = current_index * 4;
            values[valueAddr + 0] = value[0];
            values[valueAddr + 1] = value[1];
            values[valueAddr + 2] = value[2];
            values[valueAddr + 3] = value[3];
            current_index -= 1;

            if (current_index < 0) current_index = addresses.Length - 1;
        }

        byte[] result = new byte[4];
        public byte[]? GetInstruction(int address)
        {
            int pos = -1;

            #region ifs horrorosos para saber se o array de endereços contem este endereço
            if (addresses[0] == address) { pos = 0; }
            else if (addresses[1] == address) { pos = 1; } 
            else if (addresses[2] == address) { pos = 2; } 
            else if (addresses[3] == address) { pos = 3; }
            else if (addresses[4] == address) { pos = 4; }
            else if (addresses[5] == address) { pos = 5; } 
            else if (addresses[6] == address) { pos = 6; }
            else if (addresses[7] == address) { pos = 7; } 
            else if (addresses[8] == address) { pos = 8; } 
            else if (addresses[9] == address) { pos = 9; } 
            else if (addresses[10] == address) { pos = 10; }
            else if (addresses[11] == address) { pos = 11; } 
            else if (addresses[12] == address) { pos = 12; }
            else if (addresses[13] == address) { pos = 13; } 
            else if (addresses[14] == address) { pos = 14; } 
            else if (addresses[15] == address) { pos = 15; } 
            else if (addresses[16] == address) { pos = 16; } 
            else if (addresses[17] == address) { pos = 17; } 
            else if (addresses[18] == address) { pos = 18; } 
            else if (addresses[19] == address) { pos = 19; } 
            else if (addresses[20] == address) { pos = 20; }
            else if (addresses[21] == address) { pos = 21; } 
            else if (addresses[22] == address) { pos = 22; }
            else if (addresses[23] == address) { pos = 23; }
            else if (addresses[24] == address) { pos = 24; } 
            else if (addresses[25] == address) { pos = 25; }
            else if (addresses[26] == address) { pos = 26; } 
            else if (addresses[27] == address) { pos = 27; } 
            else if (addresses[28] == address) { pos = 28; } 
            else if (addresses[29] == address) { pos = 29; } 
            else if (addresses[30] == address) { pos = 30; }
            else if (addresses[31] == address) { pos = 31; }
            #endregion

            if (pos == -1) return null;

            // preencher o [] resultado com os dados do
            // buffer na posição relativa ao address
            result[0] = values[(pos * 4) + 0];
            result[1] = values[(pos * 4) + 1];
            result[2] = values[(pos * 4) + 2];
            result[3] = values[(pos * 4) + 3];

            if (pos > 0)
            {
                // promover a instrução para que fique na posição mais proxima do começo
                // quanto + vezes ela for solicitada, mais prioritária ela se torna

                // mover o valor de [values] anterior para o atual
                values[(pos * 4) + 0] = values[((pos - 1) * 4) + 0];
                values[(pos * 4) + 1] = values[((pos - 1) * 4) + 1];
                values[(pos * 4) + 2] = values[((pos - 1) * 4) + 2];
                values[(pos * 4) + 3] = values[((pos - 1) * 4) + 3];

                // mover o resultado para [values] anterior
                values[((pos - 1) * 4) + 0] = result[0];
                values[((pos - 1) * 4) + 1] = result[1];
                values[((pos - 1) * 4) + 2] = result[2];
                values[((pos - 1) * 4) + 3] = result[3];


                // promover tmb o endereço
                addresses[pos] = addresses[pos - 1];
                addresses[pos - 1] = address;
            }

            return result;
        }
    }
}
