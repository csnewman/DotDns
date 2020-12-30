using System;

namespace DotDns.Records.Caa
{
    [Flags]
    public enum CaaFlags : byte
    {
        /// <summary>
        ///     Specifies that the given property is critical. A CA must not issue certificates for the relevant domain
        ///     unless the issuer understands all critical flags. 
        /// </summary>
        IssuerCriticalFlag = 1 << 7
    }
}