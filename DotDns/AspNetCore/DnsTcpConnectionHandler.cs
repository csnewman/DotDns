using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Connections;

namespace DotDns.AspNetCore
{
    public class DnsTcpConnectionHandler : ConnectionHandler
    {
        private readonly IDnsProcessor _processor;
        private readonly IDnsReader _reader;
        private readonly IDnsWriter _writer;

        public DnsTcpConnectionHandler(IDnsProcessor processor, IDnsReader reader, IDnsWriter writer)
        {
            _processor = processor;
            _reader = reader;
            _writer = writer;
        }

        public override async Task OnConnectedAsync(ConnectionContext connection)
        {
            try
            {
                var input = connection.Transport.Input;
                var output = connection.Transport.Output;

                while (true)
                {
                    // Read length header
                    var result = await input.ReadAsync();
                    var buffer = result.Buffer;

                    if (buffer.Length < 2)
                    {
                        if (result.IsCompleted) break;

                        input.AdvanceTo(buffer.Start, buffer.End);
                        continue;
                    }

                    var length = ParseHeaderLength(ref buffer);

                    // Read body
                    while (buffer.Length < length + 2)
                    {
                        if (result.IsCompleted) break;

                        input.AdvanceTo(buffer.Start, buffer.End);
                        result = await input.ReadAsync();
                        buffer = result.Buffer;
                    }

                    // Parse request and prepare response packet
                    var requestPacket = ReadPacket(buffer.Slice(2, length));
                    input.AdvanceTo(buffer.GetPosition(length + 2));

                    var responsePacket = new DnsPacket();

                    // Process packet
                    await _processor.Process(requestPacket, responsePacket);

                    // Allocate enough memory for response based upon estimated size
                    var responseSize = _writer.CalculateSize(responsePacket) + 2;
                    using var responseMemoryOwner = MemoryPool<byte>.Shared.Rent(responseSize);
                    var responseMemory = responseMemoryOwner.Memory;

                    // Perform actual write
                    var actualLength = _writer.Write(responseMemory.Span.Slice(2), responsePacket);

                    // Fill in length header
                    BinaryPrimitives.WriteUInt16BigEndian(responseMemory.Span.Slice(0, 2), (ushort) actualLength);

                    // Perform actual write
                    await output.WriteAsync(responseMemory.Slice(0, actualLength + 2));
                }
            }
            finally
            {
                await connection.Transport.Input.CompleteAsync();
                await connection.Transport.Output.CompleteAsync();
            }
        }

        private static ushort ParseHeaderLength(ref ReadOnlySequence<byte> buffer)
        {
            var lengthSlice = buffer.Slice(0, 2);

            if (lengthSlice.IsSingleSegment) return BinaryPrimitives.ReadUInt16BigEndian(lengthSlice.First.Span);

            Span<byte> stackBuffer = stackalloc byte[4];
            lengthSlice.CopyTo(stackBuffer);
            return BinaryPrimitives.ReadUInt16BigEndian(stackBuffer);
        }

        private DnsPacket ReadPacket(ReadOnlySequence<byte> body)
        {
            if (body.Length > 8192 || body.Length == 0)
                throw new ArgumentException("Invalid packet size" + body.Length);

            if (body.IsSingleSegment) return _reader.Read(body.FirstSpan);

            using var memory = MemoryPool<byte>.Shared.Rent((int) body.Length);
            var tempBuffer = memory.Memory.Slice(0, (int) body.Length).Span;

            body.CopyTo(tempBuffer);

            return _reader.Read(tempBuffer);
        }
    }
}