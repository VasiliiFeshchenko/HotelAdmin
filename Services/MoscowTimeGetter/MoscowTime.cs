namespace MvcTest.Sevices.MoscowTimeGetter
{
    public class MoscowTime
    {
        public static DateTime Time
        {
            get
            {
                DateTimeOffset serverTime = DateTimeOffset.Now;
                TimeZoneInfo moscowTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Russian Standard Time");
                DateTimeOffset moscowTime = TimeZoneInfo.ConvertTime(serverTime, moscowTimeZone);
                DateTime moscowDateTime = moscowTime.DateTime;
                return moscowDateTime;
            }
        }
    }
}
