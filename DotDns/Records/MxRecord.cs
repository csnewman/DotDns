namespace DotDns.Records
{
    /// <summary>
    ///     MX records (mail exchanger records) specify the mail server responsible for accepting email messages on behalf
    ///     of a given domain name. A single domain name can have multiple MX records for load balancing and redundancy
    ///     purposes.
    /// </summary>
    public class MxRecord : DnsRecord
    {
        public override DnsRecordType Type => DnsRecordType.Mx;

        /// <summary>
        ///     The preference determines the order in which a client will attempt to connect to the mail server specified
        ///     in this record. Lower preferences are tried first.
        /// </summary>
        public short Preference { get; set; }

        /// <summary>
        ///     The domain name of the mail server.
        /// </summary>
        public string Exchange { get; set; }

        public MxRecord(string name, uint ttl, short preference, string exchange) : base(name, ttl)
        {
            Preference = preference;
            Exchange = exchange;
        }

        internal MxRecord(string name, uint ttl, DnsClass @class, ref DnsReadBuffer buffer, int length)
            : base(name, ttl, @class)
        {
            Preference = buffer.ReadS16();
            Exchange = buffer.ReadDomainName();
        }

        internal override void WriteData(ref DnsWriteBuffer buffer)
        {
            buffer.WriteS16(Preference);
            buffer.WriteDomainName(Exchange);
        }

        protected override int CalculateDataSize()
        {
            return 2 + DnsWriteBuffer.CalculateDomainNameSize(Exchange);
        }
    }
}