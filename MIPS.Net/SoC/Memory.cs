using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIPS.Net.SoC
{
    using System;

    public sealed class Memory
    {
        private byte[] _memory;
        public int Size { get; private set; }

        // Construtor da classe Memory
        public Memory(int memorySize)
        {
            Size = memorySize;
            _memory = new byte[memorySize]; // Inicializa a memória com o tamanho especificado
        }

        // Método para acessar um byte na memória
        public byte this[int address]
        {
            get
            {
                if (IsValidAddress(address))
                {
                    return _memory[address];
                }
                else
                {
                    throw new IndexOutOfRangeException($"Endereço de memória inválido: {address}");
                }
            }
            set
            {
                if (IsValidAddress(address))
                {
                    _memory[address] = value;
                }
                else
                {
                    throw new IndexOutOfRangeException($"Endereço de memória inválido: {address}");
                }
            }
        }

        // Método para ler um inteiro (32 bits) da memória
        public int ReadInt(int address)
        {
            if (IsValidAddress(address) && IsValidAddress(address + 3))
            {
                return BitConverter.ToInt32(_memory, address);
            }
            else
            {
                throw new IndexOutOfRangeException($"Endereço de memória inválido: {address}");
            }
        }

        // Método para escrever um inteiro (32 bits) na memória
        public void WriteInt(int address, int value)
        {
            if (IsValidAddress(address) && IsValidAddress(address + 3))
            {
                byte[] bytes = BitConverter.GetBytes(value);
                Array.Copy(bytes, 0, _memory, address, 4);
            }
            else
            {
                throw new IndexOutOfRangeException($"Endereço de memória inválido: {address}");
            }
        }

        // Método para ler uma palavra (32 bits) da memória
        public uint ReadWord(int address)
        {
            if (IsValidAddress(address) && IsValidAddress(address + 3))
            {
                return BitConverter.ToUInt32(_memory, address);
            }
            else
            {
                throw new IndexOutOfRangeException($"Endereço de memória inválido: {address}");
            }
        }

        // Método para escrever uma palavra (32 bits) na memória
        public void WriteWord(int address, uint value)
        {
            if (IsValidAddress(address) && IsValidAddress(address + 3))
            {
                byte[] bytes = BitConverter.GetBytes(value);
                Array.Copy(bytes, 0, _memory, address, 4);
            }
            else
            {
                throw new IndexOutOfRangeException($"Endereço de memória inválido: {address}");
            }
        }

        // Método para verificar se um endereço é válido
        private bool IsValidAddress(int address)
        {
            return address >= 0 && address < Size;
        }

        // Métodos para ler/escrever bytes, meio-palavras e palavras 
        // usando o endereçamento de byte para simular o MIPS

        // Ler byte
        public byte ReadByte(int address)
        {
            if (IsValidAddress(address))
            {
                return _memory[address];
            }
            else
            {
                throw new IndexOutOfRangeException($"Endereço de memória inválido: {address}");
            }
        }

        // Escrever byte
        public void WriteByte(int address, byte value)
        {
            if (IsValidAddress(address))
            {
                _memory[address] = value;
            }
            else
            {
                throw new IndexOutOfRangeException($"Endereço de memória inválido: {address}");
            }
        }

        // Ler meio-palavra (16 bits)
        public short ReadHalfword(int address)
        {
            if (IsValidAddress(address) && IsValidAddress(address + 1))
            {
                return BitConverter.ToInt16(_memory, address);
            }
            else
            {
                throw new IndexOutOfRangeException($"Endereço de memória inválido: {address}");
            }
        }

        // Escrever meio-palavra (16 bits)
        public void WriteHalfword(int address, short value)
        {
            if (IsValidAddress(address) && IsValidAddress(address + 1))
            {
                byte[] bytes = BitConverter.GetBytes(value);
                Array.Copy(bytes, 0, _memory, address, 2);
            }
            else
            {
                throw new IndexOutOfRangeException($"Endereço de memória inválido: {address}");
            }
        }

        internal void ReadBuffer(int address, ref byte[] buffer)
        {
            Array.Copy(_memory, address, buffer, 0, buffer.Length);
        }

        internal void StoreBuffer(int address, ref byte[] buffer)
        {
            Array.Copy(buffer, 0, _memory, address, buffer.Length);
        }

        internal void Reset()
        {
            for (int i = 0; i < _memory.Length; i++)
                _memory[i] = 0;
        }
    }
}
