using System;

namespace DotDns
{
    [Serializable]
    public class MalformedDnsPacketException : Exception
    {
        public MalformedDnsPacketException()
        {
        }

        public MalformedDnsPacketException(string message) : base(message)
        {
        }

        public MalformedDnsPacketException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}