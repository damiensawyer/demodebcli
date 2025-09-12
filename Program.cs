using System.Reflection;
using NodaTime;

if (args.Length > 0 && (args[0] == "--version" || args[0] == "-v"))
{
    var assembly = Assembly.GetExecutingAssembly();
    var version = assembly.GetName().Version?.ToString() ?? "Unknown";
    var buildTimeUtc = GetBuildTimeUtc();
    var localBuildTime = ConvertUtcToLocalTime(buildTimeUtc);
    
    Console.WriteLine($"demodebcli version {version}");
    Console.WriteLine($"Built: {buildTimeUtc:yyyy-MM-dd HH:mm:ss} UTC");
    Console.WriteLine($"Local time: {localBuildTime:yyyy-MM-dd HH:mm:ss}");
}
else
{
    Console.WriteLine("Hello, World - tea time");
}

static DateTime GetBuildTimeUtc()
{
    // Store build time as a compile-time constant string in UTC
    const string BuildTimeUtcString = "2025-09-12T01:55:08Z"; // This will be replaced during build
    
    if (DateTime.TryParse(BuildTimeUtcString, out var result))
    {
        return result.ToUniversalTime();
    }
    
    // Fallback to current time if parsing fails
    return DateTime.UtcNow;
}

static string ConvertUtcToLocalTime(DateTime utcTime)
{
    // Use NodaTime to convert UTC to local time
    var instant = Instant.FromDateTimeUtc(utcTime);
    var localDateTime = instant.InZone(DateTimeZoneProviders.Tzdb.GetSystemDefault()).LocalDateTime;
    return localDateTime.ToString("yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
}
