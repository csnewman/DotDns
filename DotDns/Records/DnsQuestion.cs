namespace DotDns.Records
{
    public sealed class DnsQuestion
    {
        public string Name { get; set; }

        public DnsRecordType Type { get; set; }

        public DnsClass Class { get; set; }

        public DnsQuestion(string name, DnsRecordType type, DnsClass @class = DnsClass.Internet)
        {
            Name = name;
            Type = type;
            Class = @class;
        }

        internal int CalculateSize()
        {
            return 4 + DnsWriteBuffer.CalculateDomainNameSize(Name);
        }

        public override string ToString()
        {
            return $"{nameof(Name)}: {Name}, {nameof(Type)}: {Type}, {nameof(Class)}: {Class}";
        }
    }
}