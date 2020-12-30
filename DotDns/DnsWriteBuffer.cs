using System;
using System.Buffers.Binary;
using System.Linq;
using System.Text;

namespace DotDns
{
    public ref struct DnsWriteBuffer
    {
        private readonly Span<byte> _buffer;

        public int Position { get; set; }

        public DnsWriteBuffer(Span<byte> buffer)
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

        public void WriteU8(byte value)
        {
            _buffer[Position++] = value;
        }

        public void WriteU16(ushort value)
        {
            BinaryPrimitives.WriteUInt16BigEndian(_buffer.Slice(Position, 2), value);
            Position += 2;
        }

        public void WriteS16(short value)
        {
            BinaryPrimitives.WriteInt16BigEndian(_buffer.Slice(Position, 2), value);
            Position += 2;
        }

        public void WriteU32(uint value)
        {
            BinaryPrimitives.WriteUInt32BigEndian(_buffer.Slice(Position, 4), value);
            Position += 4;
        }

        public void WriteS32(int value)
        {
            BinaryPrimitives.WriteInt32BigEndian(_buffer.Slice(Position, 4), value);
            Position += 4;
        }

        public static int CalculateDomainNameSize(string name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (name.EndsWith('.')) throw new ArgumentException("Names must not end with a '.'");

            // We currently do not support pointers
            return name.Length == 0 ? 1 : name.Count(x => x != '\\') + 2;
        }

        public void WriteDomainName(string name)
        {
            if (name.EndsWith('.'))
            {
                throw new ArgumentException("Names must not end with a '.'");
            }

            var length = 0;
            var lengthPos = Position;
            _buffer[Position++] = 0;

            if (name.Length == 0) return;

            Span<char> label = stackalloc char[63];

            var escaped = false;
            foreach (var c in name)
            {
                switch (c)
                {
                    case '\\' when !escaped:
                        escaped = true;
                        continue;
                    case '.' when !escaped:
                    {
                        var wrote = (byte) Encoding.ASCII.GetBytes(label.Slice(0, length), _buffer.Slice(Position));
                        _buffer[lengthPos] = wrote;
                        length = 0;

                        Position += wrote;
                        lengthPos = Position;
                        _buffer[Position++] = 0;
                        continue;
                    }
                    default:
                        if (length >= 63)
                        {
                            throw new ArgumentException(
                                "Each label inside a domain name must not exceed 63 characters");
                        }

                        label[length++] = c;
                        escaped = false;
                        break;
                }
            }

            if (length != 0)
            {
                var wrote = (byte) Encoding.ASCII.GetBytes(label.Slice(0, length), _buffer.Slice(Position));
                _buffer[lengthPos] = wrote;
                Position += wrote;
                _buffer[Position++] = 0;
            }
        }

        public static int CalculateCharacterStringSize(string value)
        {
            return 1 + CalculateStringSize(value);
        }

        public int WriteCharacterString(string value)
        {
            var lengthPos = Position++;
            var length = WriteFixedLengthString(value);
            _buffer[lengthPos] = (byte) (Position - lengthPos);
            return length + 1;
        }

        public int WriteFixedLengthString(string value)
        {
            var length = Encoding.ASCII.GetBytes(value, _buffer.Slice(Position));
            Position += length;
            return length;
        }

        public static int CalculateStringSize(string value)
        {
            return Encoding.ASCII.GetByteCount(value);
        }

        public Span<byte> WriteableBlob(int length)
        {
            var span = _buffer.Slice(Position, length);
            Position += length;
            return span;
        }

        public void WriteBlob(ReadOnlySpan<byte> blob)
        {
            blob.CopyTo(WriteableBlob(blob.Length));
        }

        public DnsWriteBuffer WithPosition(int position)
        {
            return new(_buffer)
            {
                Position = position
            };
        }

        public DnsWriteBuffer Slice(int start, int length, bool keepPosition = false)
        {
            var buffer = new DnsWriteBuffer(_buffer.Slice(start, length));

            if (keepPosition) buffer.Position = Position - start;

            return buffer;
        }
    }
}