using System.Globalization;

namespace Rentify.Backend.Core.Application.Modules.Shared.Helpers
{
    public static class EmailDateFormatter
    {
        private static readonly CultureInfo DominicanCulture =
            CultureInfo.GetCultureInfo("es-DO");

        private static readonly TimeZoneInfo DominicanTimeZone =
            GetDominicanTimeZone();

        public static string FormatDate(DateTime value)
        {
            var utcDate = value.Kind switch
            {
                DateTimeKind.Utc => value,
                DateTimeKind.Local => value.ToUniversalTime(),

                DateTimeKind.Unspecified =>
                    DateTime.SpecifyKind(value, DateTimeKind.Utc),

                _ => value
            };

            var dominicanDate = TimeZoneInfo.ConvertTimeFromUtc(
                utcDate,
                DominicanTimeZone);

            return dominicanDate.ToString(
                "dd 'de' MMMM 'de' yyyy, h:mm tt",
                DominicanCulture);
        }

        private static TimeZoneInfo GetDominicanTimeZone()
        {
            var timeZoneId = OperatingSystem.IsWindows()
                ? "SA Western Standard Time"
                : "America/Santo_Domingo";

            return TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
        }
    }
}
