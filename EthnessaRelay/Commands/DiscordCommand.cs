using Discord;
using Discord.Net;
using Discord.WebSocket;
using Newtonsoft.Json;

namespace EthnessaRelay.Commands;

public abstract class DiscordCommand : IDiscordCommand
{
    public string Name { get; set; }
    public string Description { get; set; }

    private readonly DiscordSocketClient _client;
    private readonly SocketTextChannel _channel;

    protected DiscordCommand(DiscordSocketClient client, SocketTextChannel channel)
    {
        _client = client;
        _channel = channel;

    }

    public virtual Task CommandHandler(SocketSlashCommand command)
    {
        throw new NotImplementedException();
    }

    public async Task AddCommand(SlashCommandBuilder command)
    {
        var guild = _channel.Guild;

        try
        {
            await guild.CreateApplicationCommandAsync(command.Build());
        }
        catch (ApplicationCommandException exception)
        {
            // If our command was invalid, we should catch an ApplicationCommandException. This exception contains the path of the error as well as the error message. You can serialize the Error field in the exception to get a visual of where your error is.
            var json = JsonConvert.SerializeObject(exception.Errors, Formatting.Indented);

            // You can send this error somewhere or just print it to the console, for this example we're just going to print it.
            Console.WriteLine(json);
        }
    }

    public virtual async Task BuildCommand()
    {
        var command = new SlashCommandBuilder()
            .WithName(Name)
            .WithDescription(Description);

        await AddCommand(command);
    }

}
