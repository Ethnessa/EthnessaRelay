using Discord;
using Discord.WebSocket;

namespace EthnessaRelay.Commands;

public interface IDiscordCommand
{
    public string Name { get; set; }
    public string Description { get; set; }
    
    public Task CommandHandler(SocketSlashCommand command);
    public Task BuildCommand();
    public Task AddCommand(SlashCommandBuilder command);
}