using System.Threading.Tasks;
using Microsoft.AspNetCore.Connections;

namespace DotDns.AspNetCore
{
    public class DnsUdpConnectionHandler : ConnectionHandler
    {
        private readonly IDnsProcessor _processor;
        private readonly IDnsReader _reader;
        private readonly IDnsWriter _writer;

        public DnsUdpConnectionHandler(IDnsProcessor processor, IDnsReader reader, IDnsWriter writer)
        {
            _processor = processor;
            _reader = reader;
            _writer = writer;
        }

        public override async Task OnConnectedAsync(ConnectionContext connection)
        {
            var udpConnection = (UdpConnection) connection;

            while (!connection.ConnectionClosed.IsCancellationRequested)
            {
                await udpConnection.AwaitConnectionAsync();

                var requestPacket = _reader.Read(udpConnection.GetReadBuffer());
                var responsePacket = new DnsPacket();

                await _processor.Process(requestPacket, responsePacket);

                var length = _writer.Write(udpConnection.GetWriteBuffer(), responsePacket);
                await udpConnection.WriteResponseAsync(length);
            }
        }
    }
}