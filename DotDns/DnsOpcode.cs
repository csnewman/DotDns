namespace DotDns
{
    /// <summary>
    ///     A four bit field that specifies kind of query in a message
    /// </summary>
    public enum DnsOpcode : byte
    {
        /// <summary>
        ///     A standard query
        /// </summary>
        StandardQuery = 0,

        /// <summary>
        ///     An inverse query
        /// </summary>
        InverseQuery = 1,

        /// <summary>
        ///     A server status request
        /// </summary>
        ServerStatusRequest = 2
    }
}