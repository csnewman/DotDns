using System;
using System.Linq;
using DotDns.Records;

namespace DotDns
{
    public class DnsWriter : IDnsWriter
    {
        public int CalculateSize(DnsPacket packet)
        {
            return 6 * 2 + packet.Questions.Sum(question => question.CalculateSize()) +
                   packet.Answers.Sum(record => record.CalculateSize()) +
                   packet.Authorities.Sum(record => record.CalculateSize()) +
                   packet.Additionals.Sum(record => record.CalculateSize());
        }

        public int Write(Span<byte> data, DnsPacket packet)
        {
            var buffer = new DnsWriteBuffer(data);

            buffer.WriteU16(packet.Id);
            buffer.WriteU16(packet.GetStatus());
            buffer.WriteU16((ushort) packet.Questions.Count);
            buffer.WriteU16((ushort) packet.Answers.Count);
            buffer.WriteU16((ushort) packet.Authorities.Count);
            buffer.WriteU16((ushort) packet.Additionals.Count);

            foreach (var question in packet.Questions) WriteQuestion(ref buffer, question);

            foreach (var record in packet.Answers) WriteRecord(ref buffer, record);

            foreach (var record in packet.Authorities) WriteRecord(ref buffer, record);

            foreach (var record in packet.Additionals) WriteRecord(ref buffer, record);

            return buffer.Position;
        }

        private void WriteQuestion(ref DnsWriteBuffer buffer, DnsQuestion question)
        {
            buffer.WriteDomainName(question.Name);
            buffer.WriteU16((ushort) question.Type);
            buffer.WriteU16((ushort) question.Class);
        }

        private void WriteRecord(ref DnsWriteBuffer buffer, DnsRecord record)
        {
            buffer.WriteDomainName(record.Name);
            buffer.WriteU16((ushort) record.Type);
            buffer.WriteU16((ushort) record.Class);
            buffer.WriteU32(record.Ttl);

            var lengthPos = buffer.Position;
            buffer.WriteU16(0);

            record.WriteData(ref buffer);

            var length = buffer.Position - lengthPos - 2;
            buffer.WithPosition(lengthPos).WriteU16((ushort) length);
        }
    }
}