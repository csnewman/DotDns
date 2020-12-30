namespace DotDns.Records
{
    public abstract class DnsRecord
    {
        public abstract DnsRecordType Type { get; }

        public string Name { get; }
        public DnsClass Class { get; protected set; }
        public virtual uint Ttl { get; }

        protected DnsRecord(string name, uint ttl, DnsClass @class = DnsClass.Internet)
        {
            Name = name;
            Class = @class;
            Ttl = ttl;
        }

        internal abstract void WriteData(ref DnsWriteBuffer buffer);

        internal int CalculateSize()
        {
            return 8 + DnsWriteBuffer.CalculateDomainNameSize(Name) + CalculateDataSize();
        }

        protected abstract int CalculateDataSize();
    }
}