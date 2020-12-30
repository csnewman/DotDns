using System;

namespace DotDns
{
    public interface IDnsWriter
    {
        int CalculateSize(DnsPacket packet);

        int Write(Span<byte> data, DnsPacket packet);
    }
}