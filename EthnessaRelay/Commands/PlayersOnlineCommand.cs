using Discord.Interactions;
using Discord.WebSocket;
using EthnessaAPI;
using Terraria;

namespace EthnessaRelay.Commands;

public class PlayersOnlineCommand : DiscordCommand
{
    public PlayersOnlineCommand(DiscordSocketClient client, SocketTextChannel channel) : base(client, channel)
    {
        Name = "players";
        Description = "Get the number of players online";
    }

    public override async Task CommandHandler(SocketSlashCommand command)
    {
        await command.RespondAsync(ServerBase.Utils.GetActivePlayerCount().ToString());
    }
}