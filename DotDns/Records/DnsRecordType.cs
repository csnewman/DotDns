using System;

namespace DotDns.Records
{
    public enum DnsRecordType : ushort
    {
        /// <summary>
        ///     Host Address
        /// </summary>
        A = 1,

        /// <summary>
        ///     Authoritative Name Server
        /// </summary>
        Ns = 2,

        /// <summary>
        ///     Mail Destination (Obsolete - use MX)
        /// </summary>
        [Obsolete("Use MX")]
        Md = 3,

        /// <summary>
        ///     Mail Forwarder (Obsolete - use MX)
        /// </summary>
        [Obsolete("Use MX")]
        Mf = 4,

        /// <summary>
        ///     Alias
        /// </summary>
        CName = 5,

        /// <summary>
        ///     Start of authority for zone
        /// </summary>
        Soa = 6,

        /// <summary>
        ///     Mailbox domain name
        /// </summary>
        [Obsolete("Unsupported")]
        Mb = 7,

        /// <summary>
        ///     Mail group member
        /// </summary>
        [Obsolete("Unsupported")]
        Mg = 8,

        /// <summary>
        ///     Mail rename domain nam
        /// </summary>
        [Obsolete("Unsupported")]
        Mr = 9,

        /// <summary>
        ///     Null
        /// </summary>
        [Obsolete("Unsupported")]
        Null = 10,

        /// <summary>
        ///     Well Known Service Description
        /// </summary>
        [Obsolete("Unsupported")]
        Wks = 11,

        /// <summary>
        ///     Domain Name Pointer
        /// </summary>
        Ptr = 12,

        /// <summary>
        ///     Host Information
        /// </summary>
        HInfo = 13,

        /// <summary>
        ///     Mailbox Information
        /// </summary>
        [Obsolete("Unsupported")]
        MInfo = 14,

        /// <summary>
        ///     Mail Exchange
        /// </summary>
        Mx = 15,

        /// <summary>
        ///     Text
        /// </summary>
        Txt = 16,

        Aaaa = 28,
        Opt = 41,

        /// <summary>
        ///     Zone Transfer
        /// </summary>
        Axfr = 252,

        /// <summary>
        ///     Request for all mailbox-related records.
        /// </summary>
        MailB = 253,

        /// <summary>
        ///     A request for all mail agent records.
        /// </summary>
        [Obsolete]
        MailA = 254,

        /// <summary>
        ///     Request for all recrods
        /// </summary>
        All = 255,

        Caa = 257
    }
}