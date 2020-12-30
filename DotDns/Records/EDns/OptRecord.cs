using System;
using System.Collections.Generic;
using System.Linq;

namespace DotDns.Records.EDns
{
    public class OptRecord : DnsRecord
    {
        public override DnsRecordType Type => DnsRecordType.Opt;

        public override uint Ttl => (uint) (ResponseCodeExtension << 24 | Version << 16 | DnsSecOK.AsInt() << 15);

        public ushort MaxUdpPayloadSize
        {
            get => (ushort) Class;
            set => Class = (DnsClass) value;
        }

        public byte ResponseCodeExtension { get; set; }

        public byte Version { get; set; }

        public bool DnsSecOK { get; set; }

        private readonly List<EDnsOption> _options;

        public OptRecord(byte version) : base("", 0)
        {
            Version = version;
            _options = new List<EDnsOption>();
        }

        internal OptRecord(string name, uint ttl, DnsClass @class, ref DnsReadBuffer buffer, int length)
            : base(name, ttl, @class)
        {
            if (name.Length != 0)
            {
                throw new MalformedDnsPacketException("Non-empty domain name for OPT record");
            }

            ResponseCodeExtension = (byte) ((ttl >> 24) & 0xFF);
            Version = (byte) ((ttl >> 16) & 0xFF);
            DnsSecOK = (ttl & (ushort) EDnsFlag.DnsSecOk) != 0;

            _options = new List<EDnsOption>();
            while (!buffer.AtEnd())
            {
                var code = (EDnsOptionCode) buffer.ReadU16();
                var optionLength = buffer.ReadU16();
                var childBuffer = buffer.Slice(0, buffer.Position + optionLength, true);
                buffer.Advance(optionLength);

                if (!childBuffer.AtEnd()) throw new ArgumentException("Option data was not fully read");

                EDnsOption option = code switch
                {
                    EDnsOptionCode.Cookie => new CookieOption(ref childBuffer, optionLength),
                    _ => throw new ArgumentOutOfRangeException(nameof(code), code, "Unknown option code")
                };

                _options.Add(option);
            }
        }

        internal override void WriteData(ref DnsWriteBuffer buffer)
        {
            foreach (var option in _options)
            {
                buffer.WriteU16((ushort) option.Code);

                var lengthPos = buffer.Position;
                buffer.WriteU16(0);

                option.WriteData(ref buffer);

                var length = buffer.Position - lengthPos - 2;
                buffer.WithPosition(lengthPos).WriteU16((ushort) length);
            }
        }

        protected override int CalculateDataSize()
        {
            return _options.Sum(option => option.CalculateSize());
        }
    }
}