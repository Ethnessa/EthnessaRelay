using Discord;
using Discord.WebSocket;

namespace EthnessaRelay.Commands;

public class CommandHandler
{
    private List<IDiscordCommand> _commands;
    
    private readonly DiscordSocketClient _client;
    private readonly SocketTextChannel _channel;
    public CommandHandler(DiscordSocketClient client, SocketTextChannel channel)
    {
        _client = client;
        _channel = channel;

        _commands = new()
        {
            new PlayersOnlineCommand(_client, _channel),
            new LoginCommand(_client, _channel)

        };
    }
    
    public async Task InstallCommandsAsync()
    {
        var guild = _channel.Guild;
        
        foreach(var cmd in _commands)
        {
            await cmd.BuildCommand();
        }
    }
}