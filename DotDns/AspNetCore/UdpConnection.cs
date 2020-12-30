using System;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Http.Features;

namespace DotDns.AspNetCore
{
    public class UdpConnection : ConnectionContext
    {
        private static readonly IPEndPoint Any = new(IPAddress.Any, IPEndPoint.MinPort);
        private static readonly IPEndPoint Ipv6Any = new(IPAddress.IPv6Any, IPEndPoint.MinPort);
        private readonly byte[] _buffer;

        private readonly Socket _listenSocket;
        private readonly SemaphoreSlim _readSemaphore;
        private readonly SocketAsyncEventArgs _recvArgs;
        private readonly SocketAsyncEventArgs _sendArgs;
        private readonly byte[] _writeBuffer;
        private readonly SemaphoreSlim _writeSemaphore;
        private int _readLength;

        public override string ConnectionId
        {
            get => "UDP";
            set { }
        }

        public override IFeatureCollection Features { get; } = new FeatureCollection();

        public override IDictionary<object, object> Items { get; set; }

        public sealed override IDuplexPipe Transport { get; set; }

        public sealed override EndPoint LocalEndPoint { get; set; }

        public sealed override EndPoint RemoteEndPoint { get; set; }

        public override CancellationToken ConnectionClosed { get; set; }

        public UdpConnection(EndPoint localEndPoint, Socket listenSocket)
        {
            LocalEndPoint = localEndPoint;
            _listenSocket = listenSocket;

            _readSemaphore = new SemaphoreSlim(0);
            _writeSemaphore = new SemaphoreSlim(0);

            _buffer = new byte[4096];
            _writeBuffer = new byte[4096];

            _recvArgs = new SocketAsyncEventArgs();
            _recvArgs.Completed += OnAsyncCompleted;
            _recvArgs.SetBuffer(_buffer);

            _sendArgs = new SocketAsyncEventArgs();
            _sendArgs.Completed += OnAsyncCompleted;
        }

        public ValueTask AwaitConnectionAsync()
        {
            _recvArgs.RemoteEndPoint = LocalEndPoint.AddressFamily == AddressFamily.InterNetworkV6 ? Ipv6Any : Any;

            return _listenSocket.ReceiveFromAsync(_recvArgs)
                ? new ValueTask(_readSemaphore.WaitAsync(ConnectionClosed))
                : default;
        }

        public ReadOnlySpan<byte> GetReadBuffer()
        {
            return new(_buffer, 0, _readLength);
        }

        public Span<byte> GetWriteBuffer()
        {
            return new(_writeBuffer);
        }

        public ValueTask WriteResponseAsync(int length)
        {
            _sendArgs.RemoteEndPoint = RemoteEndPoint;
            _sendArgs.SetBuffer(_writeBuffer, 0, length);

            return _listenSocket.SendToAsync(_sendArgs)
                ? new ValueTask(_writeSemaphore.WaitAsync(ConnectionClosed))
                : default;
        }

        private void OnAsyncCompleted(object sender, SocketAsyncEventArgs e)
        {
            switch (e.LastOperation)
            {
                case SocketAsyncOperation.ReceiveFrom:
                    if (e.SocketError == SocketError.Success)
                    {
                        RemoteEndPoint = e.RemoteEndPoint!;
                        _readLength = e.BytesTransferred;
                    }
                    else
                    {
                        throw new NotImplementedException("Handle socket error");
                    }

                    _readSemaphore.Release();
                    break;
                case SocketAsyncOperation.SendTo:
                    _writeSemaphore.Release();
                    break;
                default:
                    throw new ArgumentException("The last operation completed on the socket was not a receive or send");
            }
        }

        public override void Abort(ConnectionAbortedException abortReason)
        {
        }

        public override ValueTask DisposeAsync()
        {
            return default;
        }
    }
}