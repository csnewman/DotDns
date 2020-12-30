using System;
using System.Buffers.Binary;
using System.Text;

namespace DotDns
{
    public ref struct DnsReadBuffer
    {
        private readonly ReadOnlySpan<byte> _buffer;

        public int Position { get; set; }

        public DnsReadBuffer(ReadOnlySpan<byte> buffer)
        {
            _buffer = buffer;
            Position = 0;
        }

        public void Advance(int size)
        {
            Position += size;
        }

        public bool AtEnd()
        {
            return Position >= _buffer.Length;
        }

        public byte ReadU8()
        {
            return _buffer[Position++];
        }

        public ushort ReadU16()
        {
            var value = BinaryPrimitives.ReadUInt16BigEndian(_buffer.Slice(Position, 2));
            Position += 2;
            return value;
        }

        public short ReadS16()
        {
            var value = BinaryPrimitives.ReadInt16BigEndian(_buffer.Slice(Position, 2));
            Position += 2;
            return value;
        }

        public uint ReadU32()
        {
            var value = BinaryPrimitives.ReadUInt32BigEndian(_buffer.Slice(Position, 4));
            Position += 4;
            return value;
        }

        public int ReadS32()
        {
            var value = BinaryPrimitives.ReadInt32BigEndian(_buffer.Slice(Position, 4));
            Position += 4;
            return value;
        }

        public string ReadDomainName(int maxDepth = 10)
        {
            if (maxDepth < 0)
                throw new Exception("Error while reading domain name: max depth for decompression reached");

            var domain = new StringBuilder();
            var pointer = _buffer[Position++];

            while (pointer != 0)
            {
                var pointerType = pointer & 0b1100_0000;
                var pointerValue = pointer & 0b0011_1111;

                switch (pointerType)
                {
                    case 0b00: // Label
                        ReadFixedLengthString(pointerValue);
                        domain.Append('.');
                        pointer = _buffer[Position++];
                        break;
                    case 0b11: // Pointer
                        domain.Append(WithPosition(pointerValue).ReadDomainName(maxDepth - 1));
                        pointer = 0;
                        break;
                    default:
                        throw new MalformedDnsPacketException("Invalid domain name pointer type");
                }
            }

            if (domain.Length == 0)
            {
                return string.Empty;
            }

            domain.Length--;
            return domain.ToString();
        }

        public string ReadCharacterString()
        {
            return ReadFixedLengthString(ReadU8());
        }

        public string ReadFixedLengthString(int length)
        {
            var result = Encoding.ASCII.GetString(_buffer.Slice(Position, length));
            Position += length;
            return result;
        }

        public ReadOnlySpan<byte> ReadBlob(int length)
        {
            var span = _buffer.Slice(Position, length);
            Position += length;
            return span;
        }

        public int GetRemainingLength()
        {
            return _buffer.Length - Position;
        }

        public DnsReadBuffer WithPosition(int position)
        {
            return new(_buffer)
            {
                Position = position
            };
        }

        public DnsReadBuffer Slice(int start, int length, bool keepPosition = false)
        {
            var buffer = new DnsReadBuffer(_buffer.Slice(start, length));

            if (keepPosition) buffer.Position = Position - start;

            return buffer;
        }
    }
}