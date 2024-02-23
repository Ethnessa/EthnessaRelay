using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using EthnessaAPI;
using EthnessaRelay.Database;
using Terraria;

namespace EthnessaRelay.Commands;

public class LoginCommand : DiscordCommand
{
    public LoginCommand(DiscordSocketClient client, SocketTextChannel channel) : base(client, channel)
    {
        Name = "login";
        Description = "Allows you to login to the server as your Ethnessa server account";
        
    }

    public override async Task BuildCommand()
    {
        var command = new SlashCommandBuilder()
            .WithName(Name)
            .WithDescription(Description)
            .AddOptions(new SlashCommandOptionBuilder()
                .WithName("username")
                .WithDescription("Your Ethnessa username")
                .WithType(ApplicationCommandOptionType.String)
                .WithRequired(true),
                new SlashCommandOptionBuilder()
                    .WithName("password")
                    .WithDescription("Your Ethnessa password")
                    .WithType(ApplicationCommandOptionType.String)
                    .WithRequired(true));

        
        await AddCommand(command);
    
    }

    public override async Task CommandHandler(SocketSlashCommand command)
    {
        var result = await UserAuthentication.AttemptUserLink(
            command.User.Id, 
            (string)command.Data.Options.ElementAtOrDefault(0)?.Value, 
            (string)command.Data.Options.ElementAtOrDefault(1)?.Value);

        if (result is true)
        {
            await command.RespondAsync("You have been successfully linked to your Ethnessa account.");
        }
        else
        {
            await command.RespondAsync("Failed to link your account. Please check your username and password and try again.");
        }
    }
}