namespace DotDns.Records
{
    /// <summary>
    ///     Start of authority record.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         SOA records should always have a TTL of 0 to prevent caching.
    ///     </para>
    /// </remarks>
    public class SoaRecord : DnsRecord
    {
        public override DnsRecordType Type => DnsRecordType.Soa;

        /// <summary>
        ///     The domain name of the name server that was the original or primary source of data for this zone.
        /// </summary>
        public string PrimaryNameServer { get; }

        /// <summary>
        ///     The mailbox of the person responsible for this zone.
        /// </summary>
        public string ResponsiblePerson { get; }

        /// <summary>
        ///     The version number  of the zone. Zone transfers preserve this value. This value wraps and should be
        ///     compared using sequence space arithmetic.
        /// </summary>
        public uint Serial { get; }

        /// <summary>
        ///     The time interval in seconds before the zone should be refreshed.
        /// </summary>
        public int Refresh { get; }

        /// <summary>
        ///     The time interval in seconds that should elapse before a failed refresh should be retried.
        /// </summary>
        public int Retry { get; }

        /// <summary>
        ///     The he upper limit on the time interval that can elapse before the zone is no longer authoritative.
        /// </summary>
        public int Expire { get; }

        /// <summary>
        ///     The minimum TTL value for any record in this zone.
        /// </summary>
        public uint Minimum { get; }

        public SoaRecord(string name, string primaryNameServer, string responsiblePerson, uint serial,
            int refresh, int retry, int expire, uint minimum) : base(name, 0)
        {
            PrimaryNameServer = primaryNameServer;
            ResponsiblePerson = responsiblePerson;
            Serial = serial;
            Refresh = refresh;
            Retry = retry;
            Expire = expire;
            Minimum = minimum;
        }

        internal SoaRecord(string name, uint ttl, DnsClass @class, ref DnsReadBuffer buffer, int length)
            : base(name, ttl, @class)
        {
            PrimaryNameServer = buffer.ReadDomainName();
            ResponsiblePerson = buffer.ReadDomainName();
            Serial = buffer.ReadU32();
            Refresh = buffer.ReadS32();
            Retry = buffer.ReadS32();
            Expire = buffer.ReadS32();
            Minimum = buffer.ReadU32();
        }

        internal override void WriteData(ref DnsWriteBuffer buffer)
        {
            buffer.WriteDomainName(PrimaryNameServer);
            buffer.WriteDomainName(ResponsiblePerson);
            buffer.WriteU32(Serial);
            buffer.WriteS32(Refresh);
            buffer.WriteS32(Retry);
            buffer.WriteS32(Expire);
            buffer.WriteU32(Minimum);
        }

        protected override int CalculateDataSize()
        {
            return 4 * 5 + DnsWriteBuffer.CalculateDomainNameSize(PrimaryNameServer) +
                   DnsWriteBuffer.CalculateDomainNameSize(ResponsiblePerson);
        }
    }
}