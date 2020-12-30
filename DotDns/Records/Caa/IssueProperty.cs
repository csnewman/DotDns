using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotDns.Records.Caa
{
    /// <summary>
    ///     Grant authorization to issue certificates to the holder of the given Issuer Domain Name.
    /// </summary>
    public class IssueProperty : CaaProperty
    {
        public override string Tag => "issue";

        /// <summary>
        ///     The issuer that has authority to issue certificates.
        /// </summary>
        public string IssuerDomainName { get; set; }

        private readonly Dictionary<string, string> _parameters;

        public IssueProperty(byte flags, ref DnsReadBuffer buffer) : base(flags)
        {
            var value = buffer.ReadFixedLengthString(buffer.GetRemainingLength());
            var parts = value.Split(';');
            IssuerDomainName = parts[0];

            _parameters = new Dictionary<string, string>();
            for (var i = 1; i < parts.Length; i++)
            {
                var part = parts[i];
                var pair = part.Split('=', 2).Select(x => x.Trim()).ToArray();
                if (pair.Length != 2)
                {
                    throw new MalformedDnsPacketException(
                        "Issue property parameter did not have a key-value pair format"
                    );
                }

                _parameters.Add(pair[0], pair[1]);
            }
        }

        public IssueProperty(string issuerDomainName)
        {
            IssuerDomainName = issuerDomainName;
        }

        public void SetParameter(string key, string value)
        {
            _parameters[key] = value;
        }

        public string GetParameter(string key)
        {
            return _parameters[key];
        }

        public IEnumerable<string> GetParameters()
        {
            return _parameters.Keys;
        }

        internal override void WriteValue(ref DnsWriteBuffer buffer)
        {
            buffer.WriteFixedLengthString(ToValueString());
        }

        private string ToValueString()
        {
            if (_parameters.Count == 0)
            {
                return IssuerDomainName.Length == 0 ? ";" : IssuerDomainName;
            }

            var builder = new StringBuilder();
            builder.Append(IssuerDomainName);

            foreach (var (key, value) in _parameters)
            {
                builder.Append("; ");
                builder.Append(key);
                builder.Append(" = ");
                builder.Append(value);
            }

            return builder.ToString();
        }

        internal override int CalculateValueSize()
        {
            return DnsWriteBuffer.CalculateStringSize(ToValueString());
        }
    }
}