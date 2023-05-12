using Serilog;
using Serilog.Events;
using Serilog.Formatting.Json;

public static class LogConfiguration
{
    public static void ConfigureLogging()
    {
        string logsFolderPath = "Logs"; // Set the path to your logs folder

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .WriteTo.File(new JsonFormatter(), $"{logsFolderPath}/log.json")
            .CreateLogger();
    }
}
