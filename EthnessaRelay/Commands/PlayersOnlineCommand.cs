using Discord.Interactions;
using Discord.WebSocket;
using EthnessaAPI;
using System.Text;
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
        List<string> players = new();
        foreach(var player in ServerBase.Players)
        {
            if (player is null) continue;
            players.Add(player.Name);
        }
        StringBuilder formattedResponse = new();
        formattedResponse.AppendLine("**There are currently " + ServerBase.Utils.GetActivePlayerCount().ToString() + " players online.** ");
        formattedResponse.AppendLine(string.Join(',', players));
        await command.RespondAsync(formattedResponse.ToString());
    }
}