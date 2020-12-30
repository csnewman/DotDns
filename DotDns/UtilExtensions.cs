namespace DotDns
{
    internal static class UtilExtensions
    {
        public static int AsInt(this bool value)
        {
            return value ? 1 : 0;
        }
    }
}