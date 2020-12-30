using System;

namespace DotDns.Records.EDns
{
    [Flags]
    public enum EDnsFlag : ushort
    {
        DnsSecOk = 1 << 15
    }
}