using System.Text.RegularExpressions;

namespace DroneFlightLog.Data.Extensions
{
    public static class DroneLogStringExtensions
    {
        public static string CleanString(this string input)
        {
            return Regex.Replace(input, @"\t|\n|\r", "").Replace("  ", " ").Trim();
        }
    }
}
