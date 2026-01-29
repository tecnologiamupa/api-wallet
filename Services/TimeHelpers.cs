using System.Globalization;

namespace WalletGoogle.Services
{
    public static class TimeHelpers
    {
        public static (DateTimeOffset start, DateTimeOffset end) ParsePanamaWindow(DateOnly date, string range)
        {
            // Admite "12:00PM a 4:00PM", "8 am a 12 pm", etc.
            var parts = range.Split('a', 'A');
            if (parts.Length < 2) throw new ArgumentException("Rango inválido. Use 'HH:MM AM a HH:MM PM'.");

            var formats = new[] { "h:mm tt", "h tt", "hh:mm tt", "hh tt" };
            var culture = CultureInfo.GetCultureInfo("es-PA");

            var startT = DateTime.ParseExact(parts[0].Trim().ToUpper().Replace("AM", "AM").Replace("PM", "PM"), formats, culture, DateTimeStyles.None);
            var endT = DateTime.ParseExact(parts[1].Trim().ToUpper().Replace("AM", "AM").Replace("PM", "PM"), formats, culture, DateTimeStyles.None);

            var tz = TimeZoneInfo.FindSystemTimeZoneById("America/Panama"); // UTC-5 sin DST
            var startLocal = new DateTime(date.Year, date.Month, date.Day, startT.Hour, startT.Minute, 0, DateTimeKind.Unspecified);
            var endLocal = new DateTime(date.Year, date.Month, date.Day, endT.Hour, endT.Minute, 0, DateTimeKind.Unspecified);

            return (new DateTimeOffset(startLocal, tz.GetUtcOffset(startLocal)),
                    new DateTimeOffset(endLocal, tz.GetUtcOffset(endLocal)));
        }

        public static string Iso(DateTimeOffset dto) => dto.ToString("yyyy-MM-dd'T'HH:mm:ssK");
    }
}
