using Discord;
using Discord.WebSocket;
using EthnessaAPI;
using EthnessaRelay.Database;

namespace EthnessaRelay.Commands
{
    public class ExecuteCommand : DiscordCommand
    {
        public ExecuteCommand(DiscordSocketClient client, SocketTextChannel channel) : base(client, channel)
        {
            Name = "execute";
            Description = "Execute a command on the server";
        }

        public override async Task BuildCommand()
        {
            var command = new SlashCommandBuilder()
                .WithName(Name)
                .WithDescription(Description)
                .AddOption(new SlashCommandOptionBuilder()
                .WithName("command")
                .WithDescription("The command to execute")
                .WithType(ApplicationCommandOptionType.String)
                .WithRequired(true));

            await AddCommand(command);
        }

        //TODO: Run command as user
        public override async Task CommandHandler(SocketSlashCommand command)
        {
            var cmd = (string)command.Data.Options.ElementAtOrDefault(0)?.Value;
            var cmdSplit = cmd.Split(' ');

            if (cmd is null)
            {
                await command.RespondAsync("No command provided");
                return;
            }

            var userAccount = await UserAuthentication.GetUserAccountFromDiscord(command.User.Id);
            if (userAccount is null)
            {
                await command.RespondAsync("You must link your Ethnessa account to use this command. Use /login to link your account.");
                return;
            }

            var ethnessaCommand = EthnessaAPI.Commands.ServerCommands.FirstOrDefault(x => x.Names.Contains(cmdSplit.ElementAtOrDefault(0) ?? ""));

            if (ethnessaCommand is null)
            {
                await command.RespondAsync("Command not found");
                return;
            }

            var hasPerm = false;
            foreach (var perm in ethnessaCommand.Permissions)
            {
                if (userAccount.HasPermission(perm))
                {
                    hasPerm = true;
                    break;
                }
            }

            if (!hasPerm)
            {
                await command.RespondAsync("You do not have permission to use this command");
                return;
            }

            var result = EthnessaAPI.Commands.HandleCommand(ServerPlayer.ServerConsole, "/" + cmd);
            if (result is true)
            {
                await command.RespondAsync("Command executed successfully");
            }
            else
            {
                await command.RespondAsync("Failed to execute command");
            }
        }
    }
}
