namespace DotDns.Records
{
    /// <summary>
    ///     Ptr records provide the Domain Name associated with a given IP. They are the inverse of
    ///     <see cref="ARecord" />/<see cref="AaaaRecord" />.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         IPV4 addresses, such as <c>1.2.3.4</c> have their reverse stored under <c>4.3.2.1.in-addr.arpa</c>.
    ///     </para>
    ///     <para>
    ///         IPV6 addresses have their reverse stored under <c>{reversed ipv6}.ip6.arpa</c>.
    ///     </para>
    /// </remarks>
    public class PtrRecord : DnsRecord
    {
        public override DnsRecordType Type => DnsRecordType.Ptr;

        /// <summary>
        ///     The canonical domain name for the given resource.
        /// </summary>
        public string Domain { get; }

        public PtrRecord(string name, uint ttl, string domain) : base(name, ttl)
        {
            Domain = domain;
        }

        internal PtrRecord(string name, uint ttl, DnsClass @class, ref DnsReadBuffer buffer, int length)
            : base(name, ttl, @class)
        {
            Domain = buffer.ReadDomainName();
        }

        internal override void WriteData(ref DnsWriteBuffer buffer)
        {
            buffer.WriteDomainName(Domain);
        }

        protected override int CalculateDataSize()
        {
            return DnsWriteBuffer.CalculateDomainNameSize(Domain);
        }
    }
}