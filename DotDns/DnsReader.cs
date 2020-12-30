using System;
using DotDns.Records;
using DotDns.Records.Caa;
using DotDns.Records.EDns;

namespace DotDns
{
    public class DnsReader : IDnsReader
    {
        public DnsPacket Read(ReadOnlySpan<byte> data)
        {
            var buffer = new DnsReadBuffer(data);

            var packet = new DnsPacket();
            packet.Id = buffer.ReadU16();

            var bits = buffer.ReadU16();
            packet.ParseStatus(bits);

            var qdCount = buffer.ReadU16();
            var anCount = buffer.ReadU16();
            var nsCount = buffer.ReadU16();
            var arCount = buffer.ReadU16();

            for (var i = 0; i < qdCount; i++) packet.AddQuestion(ReadQuestion(ref buffer));

            for (var i = 0; i < anCount; i++) packet.AddAnswer(ReadRecord(ref buffer));

            for (var i = 0; i < nsCount; i++) packet.AddAuthority(ReadRecord(ref buffer));

            for (var i = 0; i < arCount; i++) packet.AddAdditional(ReadRecord(ref buffer));

            return packet;
        }

        private DnsQuestion ReadQuestion(ref DnsReadBuffer buffer)
        {
            var name = buffer.ReadDomainName();

            if (name == null) throw new Exception("name was null");

            var type = (DnsRecordType) buffer.ReadU16();
            var @class = (DnsClass) buffer.ReadU16();

            return new DnsQuestion(name, type, @class);
        }

        private DnsRecord ReadRecord(ref DnsReadBuffer buffer)
        {
            var name = buffer.ReadDomainName();
            var type = (DnsRecordType) buffer.ReadU16();
            var @class = (DnsClass) buffer.ReadU16();
            var ttl = buffer.ReadU32();
            var length = buffer.ReadU16();
            var childBuffer = buffer.Slice(0, buffer.Position + length, true);
            buffer.Advance(length);

            DnsRecord record = type switch
            {
                DnsRecordType.A => new ARecord(name, ttl, @class, ref childBuffer, length),
                DnsRecordType.Soa => new SoaRecord(name, ttl, @class, ref childBuffer, length),
                DnsRecordType.Ptr => new PtrRecord(name, ttl, @class, ref childBuffer, length),
                DnsRecordType.Txt => new TxtRecord(name, ttl, @class, ref childBuffer, length),
                DnsRecordType.Aaaa => new AaaaRecord(name, ttl, @class, ref childBuffer, length),
                DnsRecordType.Caa => new CaaRecord(name, ttl, @class, ref childBuffer, length),
                DnsRecordType.Opt => new OptRecord(name, ttl, @class, ref childBuffer, length),
                DnsRecordType.Ns => new NsRecord(name, ttl, @class, ref childBuffer, length),
                DnsRecordType.HInfo => new HInfoRecord(name, ttl, @class, ref childBuffer, length),
                DnsRecordType.Mx => new MxRecord(name, ttl, @class, ref childBuffer, length),
                DnsRecordType.CName => new CNameRecord(name, ttl, @class, ref childBuffer, length),
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, "Unknown record type")
            };

            if (!childBuffer.AtEnd()) throw new ArgumentException("Record data was not fully read");

            return record;
        }
    }
}