namespace QAutomation.Extensions
{
    public static class ObjectExtensions
    {
        public static bool IsNull(this object o)
        {
            return o == null;
        }

        public static bool NotNull(this object o)
        {
            return !IsNull(o);
        }
    }
}