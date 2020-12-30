namespace DotDns.Records.EDns
{
    /// <summary>
    ///     DNS Cookies are a lightweight DNS transaction security mechanism that provides limited protection to DNS
    ///     servers and clients against a variety of increasingly common denial-of-service and amplification/ forgery or
    ///     cache poisoning attacks by off-path attackers.
    /// </summary>
    public class CookieOption : EDnsOption
    {
        public override EDnsOptionCode Code => EDnsOptionCode.Cookie;

        public byte[] ClientCookie { get; set; }

        public byte[]? ServerCookie { get; set; }

        public CookieOption(byte[] clientCookie, byte[]? serverCookie)
        {
            ClientCookie = clientCookie;
            ServerCookie = serverCookie;
        }

        public CookieOption(ref DnsReadBuffer buffer, int length)
        {
            var serverCookieLength = length - 8;
            if (serverCookieLength > 32)
            {
                throw new MalformedDnsPacketException("Server cookie larger than 32 bytes");
            }

            ClientCookie = buffer.ReadBlob(8).ToArray();

            if (serverCookieLength > 0)
            {
                ServerCookie = buffer.ReadBlob(serverCookieLength).ToArray();
            }
        }

        internal override void WriteData(ref DnsWriteBuffer buffer)
        {
            buffer.WriteBlob(ClientCookie);

            if (ServerCookie != null)
            {
                buffer.WriteBlob(ServerCookie);
            }
        }

        protected override int CalculateDataSize()
        {
            return ClientCookie.Length + (ServerCookie?.Length ?? 0);
        }
    }
}