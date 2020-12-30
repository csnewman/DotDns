namespace DotDns.Records.Caa
{
    /// <summary>
    ///     The iodef Property specifies a means of reporting certificate issue requests which issuances violate the
    ///     security policy of the Issuer or the FQDN holder.
    /// </summary>
    public class IoDefProperty : CaaProperty
    {
        public override string Tag => "iodef";

        /// <summary>
        ///     The location where to report security policy violations. The most common formats are:
        ///     "mailto:security@example.com" and "https://iodef.example.com/"
        /// </summary>
        public string ReportLocation { get; set; }

        public IoDefProperty(byte flags, ref DnsReadBuffer buffer) : base(flags)
        {
            ReportLocation = buffer.ReadFixedLengthString(buffer.GetRemainingLength());
        }

        public IoDefProperty(string reportLocation)
        {
            ReportLocation = reportLocation;
        }

        internal override void WriteValue(ref DnsWriteBuffer buffer)
        {
            buffer.WriteFixedLengthString(ReportLocation);
        }

        internal override int CalculateValueSize()
        {
            return DnsWriteBuffer.CalculateStringSize(ReportLocation);
        }
    }
}