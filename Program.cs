using System.Reflection;

if (args.Length > 0 && (args[0] == "--version" || args[0] == "-v"))
{
    var assembly = Assembly.GetExecutingAssembly();
    var version = assembly.GetName().Version?.ToString() ?? "Unknown";
    var buildDate = GetBuildDate(assembly);
    
    Console.WriteLine($"demodebcli version {version}");
    Console.WriteLine($"Built: {buildDate:yyyy-MM-dd HH:mm:ss} UTC");
    Console.WriteLine($"Local time: {buildDate.ToLocalTime():yyyy-MM-dd HH:mm:ss}");
}
else
{
    Console.WriteLine("Hello, World!");
}

static DateTime GetBuildDate(Assembly assembly)
{
    const string BuildVersionMetadataPrefix = "+build";
    var attribute = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
    if (attribute?.InformationalVersion != null)
    {
        var value = attribute.InformationalVersion;
        var index = value.IndexOf(BuildVersionMetadataPrefix);
        if (index > 0)
        {
            value = value.Substring(index + BuildVersionMetadataPrefix.Length);
            if (DateTime.TryParse(value, out var result))
                return result;
        }
    }
    
    try
    {
        var baseDir = AppContext.BaseDirectory;
        if (!string.IsNullOrEmpty(baseDir))
        {
            return File.GetLastWriteTimeUtc(baseDir);
        }
    }
    catch { }
    
    return DateTime.UtcNow;
}
