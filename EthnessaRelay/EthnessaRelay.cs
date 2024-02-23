using Discord;
using Discord.WebSocket;
using EthnessaAPI;
using EthnessaAPI.Hooks;
using EthnessaRelay.Commands;
using EthnessaRelay.Configuration;
using Terraria;
using TerrariaApi.Server;

namespace EthnessaRelay
{
    [ApiVersion(2,1)]
    public class EthnessaRelay : TerrariaPlugin
    {
        public override string Name => "Ethnessa Relay";
        public override string Author => "Average";
        public override string Description => "Relays server messages to a discord channel, and (will) facilitates cross-server communication";
        public override Version Version => new(1,0,0,0);
        
        private DiscordSocketClient client;
        private SocketTextChannel? channel;
        public static RelaySettings Config { get; set; }
        
        public EthnessaRelay(Main game) : base(game)
        {

        }

        public override void Initialize()
        {
            Config = RelaySettings.Load();
            
            // we are initializing the bot in the OnServerLoaded method, because we want to prevent the bot from sending messages before the world has been loaded
            ServerApi.Hooks.GamePostInitialize.Register(this, OnServerLoaded);
            EthnessaAPI.Hooks.PlayerHooks.PlayerChat += OnPlayerChat;
            ServerApi.Hooks.NetGreetPlayer.Register(this, OnPlayerJoin);
            ServerApi.Hooks.ServerLeave.Register(this,OnPlayerLeave);
        }

        private void OnPlayerJoin(GreetPlayerEventArgs args)
        {
            if(channel is not null)
            {
                channel.SendMessageAsync($"{args.Who} has joined the server");
            }
        }



        private void OnPlayerChat(PlayerChatEventArgs args)
        {
            if(channel is not null)
            {
                channel.SendMessageAsync(args.TShockFormattedText);
            }
        }

        private void OnServerLoaded(EventArgs args)
        {
            client = new DiscordSocketClient();
            client.Log += RelayLogger.DiscordLog;

            Task.Run(BotAsync);
        }

        private async Task BotAsync()
        {
            await client.LoginAsync(TokenType.Bot, Config.Token);
            await client.StartAsync();

            while (client.ConnectionState != ConnectionState.Connected)
            {
                await Task.Delay(1000);
            }
            
            // set main channel
            channel = await client.GetChannelAsync(Config.ChannelId) as SocketTextChannel;
            
            if (channel is null)
            {
                ServerBase.Log.ConsoleError($"Channel ID {Config.ChannelId} is invalid");
                this.Dispose();
                return;
            }

            CommandHandler commandHandler = new(client, channel);
            await commandHandler.InstallCommandsAsync();
            
            await Task.Delay(-1);
        }

    }
}
