using System.Linq;

namespace DotDns.Records
{
    /// <summary>
    /// Text records are used to hold descriptive text.  The semantics of the text depends on the domain where it is found.
    /// </summary>
    public class TxtRecord : DnsRecord
    {
        public override DnsRecordType Type => DnsRecordType.Txt;

        public string[] Data { get; }

        public TxtRecord(string name, uint ttl, string[] data) : base(name, ttl)
        {
            Data = data;
        }

        internal TxtRecord(string name, uint ttl, DnsClass @class, ref DnsReadBuffer buffer, int length)
            : base(name, ttl, @class)
        {
            var start = buffer.Position;
            var end = buffer.Position + length;
            var stringCount = 0;

            while (buffer.Position < end)
            {
                buffer.Advance(buffer.ReadU8());
                stringCount++;
            }

            buffer.Position = start;

            Data = new string[stringCount];
            for (var i = 0; i < stringCount; i++) Data[i] = buffer.ReadCharacterString();
        }

        internal override void WriteData(ref DnsWriteBuffer buffer)
        {
            foreach (var s in Data) buffer.WriteCharacterString(s);
        }

        protected override int CalculateDataSize()
        {
            return Data.Sum(DnsWriteBuffer.CalculateCharacterStringSize);
        }
    }
}