namespace DotDns.Records.EDns
{
    public abstract class EDnsOption
    {
        public abstract EDnsOptionCode Code { get; }

        protected EDnsOption()
        {
        }

        internal abstract void WriteData(ref DnsWriteBuffer buffer);

        internal int CalculateSize()
        {
            return 2 + CalculateDataSize();
        }

        protected abstract int CalculateDataSize();
    }
}