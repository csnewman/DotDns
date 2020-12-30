using System;
using System.Collections.Generic;
using System.Linq;
using DotDns.Records;
using DotDns.Records.EDns;

namespace DotDns
{
    public sealed class DnsPacket
    {
        /// <summary>
        ///     A identifier assigned by the program that generates any kind of query. This identifier is copied in the
        ///     corresponding reply and can be used by the requester to match up replies to outstanding queries.
        /// </summary>
        public ushort Id { get; set; }

        /// <summary>
        ///     Specifies whether this message is a response
        /// </summary>
        public bool IsResponse { get; set; }

        /// <summary>
        ///     Specifies kind of query in this message. This value is set by the originator of a query and copied into the
        ///     response.
        /// </summary>
        public DnsOpcode OpCode { get; set; }

        /// <summary>
        ///     Specifies whether the this response is an Authoritative Answer, meaning the responding name server is an
        ///     authority for the domain name in question section.
        ///     Note that the contents of the answer section may have multiple owner names because of aliases. This field
        ///     corresponds to the name which matches the query name, or the first owner name in the answer section.
        /// </summary>
        public bool Authoritative { get; set; }

        /// <summary>
        ///     Specifies that this message was truncated due to length being greater than that permitted on the
        ///     transmission channel.
        /// </summary>
        public bool Truncated { get; set; }

        /// <summary>
        ///     If set, it directs the name server to pursue the query recursively. Recursive query support is optional.
        ///     This bit may be set in a query and is copied into the response.
        /// </summary>
        public bool RecursionDesired { get; set; }

        /// <summary>
        ///     Set by the name server in a response, and denotes whether recursive query support is available.
        /// </summary>
        public bool RecursionAvailable { get; set; }

        /// <summary>
        ///     Reserved for future use.  Must be zero in all queries and responses.
        /// </summary>
        public bool Zero { get; set; }

        public bool AuthenticatedData { get; set; }

        public bool CheckingDisabled { get; set; }

        /// <summary>
        ///     Specifies the status of the response.
        /// </summary>
        public DnsResponseCode ResponseCode
        {
            get
            {
                int value = _responseCodeLower;
                if (OptRecord != null)
                {
                    value += OptRecord.ResponseCodeExtension << 4;
                }

                return (DnsResponseCode) value;
            }
            set
            {
                _responseCodeLower = (byte) ((ushort) value & 0xFF);
                if (OptRecord != null)
                {
                    OptRecord.ResponseCodeExtension = (byte) (((ushort) value >> 4) & 0xFF);
                }
                else if ((ushort) value > byte.MaxValue)
                {
                    throw new ArgumentException("Extended response codes require ends");
                }
            }
        }

        private byte _responseCodeLower;

        /// <summary>
        ///     A set of questions that are being asked against the name server.
        ///     Usually more than one question will be rejected. Questions are copied into the response.
        /// </summary>
        public IList<DnsQuestion> Questions { get; }

        /// <summary>
        ///     A set of direct answers to the questions.
        /// </summary>
        public IList<DnsRecord> Answers { get; }

        public IList<DnsRecord> Authorities { get; }

        public IList<DnsRecord> Additionals { get; }

        private OptRecord? _optRecordCache;

        public OptRecord? OptRecord
        {
            get { return _optRecordCache ??= GetAdditionals<OptRecord>().FirstOrDefault(); }
        }

        public DnsPacket()
        {
            Questions = new List<DnsQuestion>();
            Answers = new List<DnsRecord>();
            Authorities = new List<DnsRecord>();
            Additionals = new List<DnsRecord>();
        }

        public void ParseStatus(ushort bits)
        {
            IsResponse = (bits & (1 << 15)) != 0;
            OpCode = (DnsOpcode) ((bits >> 11) & 0xF);
            Authoritative = (bits & (1 << 10)) != 0;
            Truncated = (bits & (1 << 9)) != 0;
            RecursionDesired = (bits & (1 << 8)) != 0;
            RecursionAvailable = (bits & (1 << 7)) != 0;
            Zero = (bits & (1 << 6)) != 0;
            AuthenticatedData = (bits & (1 << 5)) != 0;
            CheckingDisabled = (bits & (1 << 4)) != 0;
            ResponseCode = (DnsResponseCode) (bits & 0xF);
        }

        public ushort GetStatus()
        {
            var value = ((byte) OpCode << 11) | ((byte) ResponseCode & 0xF);
            if (IsResponse) value |= 1 << 15;
            if (Authoritative) value |= 1 << 10;
            if (Truncated) value |= 1 << 9;
            if (RecursionDesired) value |= 1 << 8;
            if (RecursionAvailable) value |= 1 << 7;
            if (Zero) value |= 1 << 6;
            if (AuthenticatedData) value |= 1 << 5;
            if (CheckingDisabled) value |= 1 << 4;
            return (ushort) value;
        }

        public void AddQuestion(DnsQuestion question)
        {
            Questions.Add(question);
        }

        public void AddAnswer(DnsRecord record)
        {
            Answers.Add(record);
        }

        public void AddAuthority(DnsRecord record)
        {
            Authorities.Add(record);
        }

        public void AddAdditional(DnsRecord record)
        {
            Additionals.Add(record);
        }

        public IEnumerable<T> GetAnswers<T>()
        {
            return Answers.Where(x => x is T).Cast<T>();
        }

        public IEnumerable<T> GetAuthorities<T>()
        {
            return Authorities.Where(x => x is T).Cast<T>();
        }

        public IEnumerable<T> GetAdditionals<T>()
        {
            return Additionals.Where(x => x is T).Cast<T>();
        }

        public void Reset()
        {
            Id = 0;
            IsResponse = false;
            OpCode = 0;
            Authoritative = false;
            Truncated = false;
            RecursionDesired = false;
            RecursionAvailable = false;
            Zero = false;
            AuthenticatedData = false;
            CheckingDisabled = false;
            ResponseCode = DnsResponseCode.ServerFailure;
            Questions.Clear();
            Answers.Clear();
            Authorities.Clear();
            Additionals.Clear();
            _optRecordCache = null;
        }
    }
}