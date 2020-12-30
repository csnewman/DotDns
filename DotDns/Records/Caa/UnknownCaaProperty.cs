namespace DotDns.Records.Caa
{
    public class UnknownCaaProperty : CaaProperty
    {
        public override string Tag { get; }

        public byte[] Value { get; set; }

        public UnknownCaaProperty(string tag, byte[] value)
        {
            Tag = tag;
            Value = value;
        }

        public UnknownCaaProperty(string tag, byte flags, ref DnsReadBuffer buffer) : base(flags)
        {
            Tag = tag;
            Value = buffer.ReadBlob(buffer.GetRemainingLength()).ToArray();
        }

        internal override void WriteValue(ref DnsWriteBuffer buffer)
        {
            buffer.WriteBlob(Value);
        }

        internal override int CalculateValueSize()
        {
            return Value.Length;
        }
    }
}