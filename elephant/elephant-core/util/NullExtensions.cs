using System;

namespace elephant.core.util
{
    public static class NullExtensions
    {
        public static T RequireNonNull<T>(this T obj, string message) where T : class
        {
            if (obj == null)
                throw new NullReferenceException(message);
            return obj;
        }
    }
}
