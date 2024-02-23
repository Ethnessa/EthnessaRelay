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
            new LoginCommand(_client, _channel),
            new ExecuteCommand(_client, _channel)
        };

    }
    
    public async Task InstallCommandsAsync()
    {
        var guild = _channel.Guild;
        
        foreach(var cmd in _commands)
        {
            await cmd.BuildCommand();
        }

        _client.SlashCommandExecuted += HandleSlashCommand;

    }

    public async Task HandleSlashCommand(SocketSlashCommand command)
    {
        var cmd = _commands.FirstOrDefault(x => x.Name == command.Data.Name);
        if (cmd is null)
        {
            await command.RespondAsync("Command not found");
            return;
        }
        await cmd.CommandHandler(command);
    }


}