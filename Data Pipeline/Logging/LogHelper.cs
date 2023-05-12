using Serilog;

public static class LogHelper
{
    public static void LogInformation(string message)
    {
        Log.Information(message);
    }

    public static void LogWarning(string message)
    {
        Log.Warning(message);
    }

    public static void LogError(string message)
    {
        Log.Error(message);
    }

    public static void LogException(string message, System.Exception ex)
    {
        Log.Error(ex, message);
    }
}
