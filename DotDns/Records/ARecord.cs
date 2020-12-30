using System;
using System.Net;
using System.Net.Sockets;

namespace DotDns.Records
{
    /// <summary>
    /// Address records the IP Address associated with a given domain name.
    /// </summary>
    public class ARecord : DnsRecord
    {
        public override DnsRecordType Type => DnsRecordType.A;

        /// <summary>
        /// The IPv4 address of the given domain.
        /// </summary>
        public IPAddress Address { get; }

        public ARecord(string name, uint ttl, IPAddress address) : base(name, ttl)
        {
            Address = address;

            if (Address.AddressFamily != AddressFamily.InterNetwork)
                throw new Exception("Invalid IP address family.");
        }

        internal ARecord(string name, uint ttl, DnsClass @class, ref DnsReadBuffer buffer, int length)
            : base(name, ttl, @class)
        {
            Address = new IPAddress(buffer.ReadBlob(length));

            if (Address.AddressFamily != AddressFamily.InterNetworkV6)
                throw new Exception("Invalid IP address family.");
        }

        internal override void WriteData(ref DnsWriteBuffer buffer)
        {
            if (!Address.TryWriteBytes(buffer.WriteableBlob(4), out var written))
                throw new Exception("Failed to write Address");

            if (written != 4) throw new Exception("Written address had incorrect length");
        }

        protected override int CalculateDataSize()
        {
            return 4;
        }
    }
}