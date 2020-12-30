namespace DotDns.Records.Caa
{
    /// <summary>
    ///     The Certification Authority Authorization (CAA) DNS Resource Record allows a DNS domain name holder to
    ///     specify the Certification Authorities (CAs) authorized to issue certificates for that domain name.
    /// </summary>
    public class CaaRecord : DnsRecord
    {
        public override DnsRecordType Type => DnsRecordType.Caa;

        public CaaProperty Property { get; }

        public CaaRecord(string name, uint ttl, CaaProperty property) : base(name, ttl)
        {
            Property = property;
        }

        internal CaaRecord(string name, uint ttl, DnsClass @class, ref DnsReadBuffer buffer, int length)
            : base(name, ttl, @class)
        {
            var flags = buffer.ReadU8();
            var tagLength = buffer.ReadU8();
            var tag = buffer.ReadFixedLengthString(tagLength);

            Property = tag.ToLower() switch
            {
                "issue" => new IssueProperty(flags, ref buffer),
                "issuewild" => new IssueWildProperty(flags, ref buffer),
                "iodef" => new IoDefProperty(flags, ref buffer),
                _ => new UnknownCaaProperty(tag, flags, ref buffer),
            };
        }

        internal override void WriteData(ref DnsWriteBuffer buffer)
        {
            buffer.WriteU8(Property.ToFlags());
            buffer.WriteU8((byte) DnsWriteBuffer.CalculateStringSize(Property.Tag));
            buffer.WriteFixedLengthString(Property.Tag);
            Property.WriteValue(ref buffer);
        }

        protected override int CalculateDataSize()
        {
            return 2 + DnsWriteBuffer.CalculateStringSize(Property.Tag) + Property.CalculateValueSize();
        }
    }
}