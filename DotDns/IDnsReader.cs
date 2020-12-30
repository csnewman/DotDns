using System;

namespace DotDns
{
    public interface IDnsReader
    {
        DnsPacket Read(ReadOnlySpan<byte> data);
    }
}