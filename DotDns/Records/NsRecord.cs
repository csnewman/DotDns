namespace DotDns.Records
{
    /// <summary>
    ///     A name server record points to the DNS server that is authoritative for the given domain name.
    /// </summary>
    /// <remarks>
    ///     NS records must always point to the canonical domain name of the DNS server. You can never point to
    ///     a <see cref="CNameRecord" />.
    /// </remarks>
    public class NsRecord : DnsRecord
    {
        public override DnsRecordType Type => DnsRecordType.Ns;

        /// <summary>
        ///     The canonical domain name of the authoritative DNS server.
        /// </summary>
        public string Domain { get; }

        public NsRecord(string name, uint ttl, string domain) : base(name, ttl)
        {
            Domain = domain;
        }

        internal NsRecord(string name, uint ttl, DnsClass @class, ref DnsReadBuffer buffer, int length)
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