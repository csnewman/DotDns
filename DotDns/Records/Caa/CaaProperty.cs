namespace DotDns.Records.Caa
{
    public abstract class CaaProperty
    {
        public bool Critical { get; set; }

        public abstract string Tag { get; }

        protected CaaProperty()
        {
        }

        protected CaaProperty(byte flags)
        {
            Critical = (flags & (byte) CaaFlags.IssuerCriticalFlag) != 0;
        }

        internal byte ToFlags()
        {
            byte value = 0;
            if (Critical) value |= (byte) CaaFlags.IssuerCriticalFlag;
            return value;
        }

        internal abstract void WriteValue(ref DnsWriteBuffer buffer);

        internal abstract int CalculateValueSize();
    }
}