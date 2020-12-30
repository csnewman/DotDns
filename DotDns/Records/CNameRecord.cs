namespace DotDns.Records
{
    /// <summary>
    ///     CName records map one domain name (alias) to another (canonical).
    /// </summary>
    public class CNameRecord : DnsRecord
    {
        public override DnsRecordType Type => DnsRecordType.CName;

        /// <summary>
        ///     A domain name which specifies the canonical domain name.
        /// </summary>
        public string Domain { get; set; }

        public CNameRecord(string name, uint ttl, string domain) : base(name, ttl)
        {
            Domain = domain;
        }

        internal CNameRecord(string name, uint ttl, DnsClass @class, ref DnsReadBuffer buffer, int length)
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