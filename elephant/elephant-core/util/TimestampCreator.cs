namespace elephant.core.util
{
    public class TimestampCreator
    {
        public static string CreateTimestampString()
        {
            return "" + System.Convert.ToInt64(System.DateTime.UtcNow.ToUniversalTime().Subtract(new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc)).TotalMilliseconds) / 1000L;
        }
    }
}
