namespace DotDns.Records
{
    /// <summary>
    ///     HINFO records are used to acquire general information about a host. The main use is for protocols such as FTP
    ///     that can use special procedures when talking between machines or operating systems of the same type.
    /// </summary>
    public class HInfoRecord : DnsRecord
    {
        public override DnsRecordType Type => DnsRecordType.HInfo;

        /// <summary>
        ///     Specifies the CPU type of the host.
        /// </summary>
        public string Cpu { get; set; }

        /// <summary>
        ///     Specifies the OS type of the host.
        /// </summary>
        public string Os { get; set; }

        public HInfoRecord(string name, uint ttl, string cpu, string os) : base(name, ttl)
        {
            Cpu = cpu;
            Os = os;
        }

        internal HInfoRecord(string name, uint ttl, DnsClass @class, ref DnsReadBuffer buffer, int length)
            : base(name, ttl, @class)
        {
            Cpu = buffer.ReadCharacterString();
            Os = buffer.ReadCharacterString();
        }

        internal override void WriteData(ref DnsWriteBuffer buffer)
        {
            buffer.WriteCharacterString(Cpu);
            buffer.WriteCharacterString(Os);
        }

        protected override int CalculateDataSize()
        {
            return DnsWriteBuffer.CalculateCharacterStringSize(Cpu) + DnsWriteBuffer.CalculateCharacterStringSize(Os);
        }
    }
}