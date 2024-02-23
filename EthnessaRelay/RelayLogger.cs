using Discord;
using EthnessaAPI;

namespace EthnessaRelay;

public class RelayLogger
{
    public static Task DiscordLog(LogMessage msg)
    {
        if (ServerBase.Log is null)
        {
            return Task.CompletedTask;
        }
        var message = $"[EthnessaRelay] {msg.Message} {msg.Exception?.Message}";
        // theres probably an existing method for this, but I'm not sure what it is
        switch (msg.Severity)
        {
            case LogSeverity.Critical:
            case LogSeverity.Error:
            {
                ServerBase.Log.ConsoleError(message);
                break;
            }
            case LogSeverity.Info:
            {
                ServerBase.Log.ConsoleInfo(message);
                break;
            }
            case LogSeverity.Debug:
            case LogSeverity.Verbose:
            {
                ServerBase.Log.ConsoleDebug(message);
                break;
            }
            case LogSeverity.Warning:
            {
                ServerBase.Log.ConsoleWarn(message);
                break;
            }
            default:
            {
                ServerBase.Log.ConsoleInfo(message);
                break;
            }        
        }

        return Task.CompletedTask;
    }
}