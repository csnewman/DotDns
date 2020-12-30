using System;
using System.Net;
using System.Net.Sockets;

namespace DotDns.Records
{
    /// <summary>
    /// IPV6 Address records provide the IPV6 address for a given domain name.
    /// </summary>
    public class AaaaRecord : DnsRecord
    {
        public override DnsRecordType Type => DnsRecordType.Aaaa;

        /// <summary>
        /// The IPv6 address of the given domain name.
        /// </summary>
        public IPAddress Address { get; set; }

        public AaaaRecord(string name, uint ttl, IPAddress address) : base(name, ttl)
        {
            Address = address;

            if (Address.AddressFamily != AddressFamily.InterNetworkV6)
                throw new Exception("Invalid IP address family.");
        }

        internal AaaaRecord(string name, uint ttl, DnsClass @class, ref DnsReadBuffer buffer, int length)
            : base(name, ttl, @class)
        {
            Address = new IPAddress(buffer.ReadBlob(length));

            if (Address.AddressFamily != AddressFamily.InterNetworkV6)
                throw new Exception("Invalid IP address family.");
        }

        internal override void WriteData(ref DnsWriteBuffer buffer)
        {
            if (!Address.TryWriteBytes(buffer.WriteableBlob(8), out var written))
                throw new Exception("Failed to write Address");

            if (written != 8) throw new Exception("Written address had incorrect length");
        }

        protected override int CalculateDataSize()
        {
            return 8;
        }
    }
}