using Discord;
using Discord.WebSocket;
using EthnessaAPI;
using EthnessaAPI.Hooks;
using EthnessaRelay.Commands;
using EthnessaRelay.Configuration;
using EthnessaRelay.Database;
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
        
        public static DiscordSocketClient Client { get; set; }
        public static SocketTextChannel? Channel { get; set; }
        public static SocketTextChannel? LogChannel { get; set; }
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

            EthnessaAPI.GetDataHandlers.KillMe += OnPlayerDeath;
            
        }

        private void OnPlayerDeath(object? sender, GetDataHandlers.KillMeEventArgs e)
        {
            if (Config.ShowDeathMessages is false)
            {
                return;
            }
            var player = ServerBase.Players.ElementAtOrDefault(e.PlayerId);
            var embed = new EmbedBuilder()
            {
                Title=$"{Config.ServerName}",
                Description=$":skull: **{player?.Name}** has died",
                Color = new Color(174,38,38)
            }.Build();
            Channel?.SendMessageAsync(embed:embed);
        }

        private void OnPlayerJoin(GreetPlayerEventArgs args)
        {
            if (Config.ShowJoinMessages is false)
            {
                return;
            }
            
            var player = ServerBase.Players.ElementAtOrDefault(args.Who);
            var embed = new EmbedBuilder()
            {
                Title=$"{Config.ServerName}",
                Description=$":small_blue_diamond: **{player?.Name}** has joined the server",
                Color = new Color(38,72,174),
                Fields = new List<EmbedFieldBuilder>()
                {
                    new()
                    {
                        Name="Players Online:",
                        Value = $"{ServerBase.Utils.GetActivePlayerCount()} / {ServerBase.Config.Settings.MaxSlots}"
                    }
                }
            }.Build();
            
            Channel?.SendMessageAsync(embed:embed);
            UpdateStatus();
        }
        
        private void OnPlayerLeave(LeaveEventArgs args)
        {
            if (Config.ShowLeaveMessages is false)
            {
                return;
            }
            
            var player = ServerBase.Players.ElementAtOrDefault(args.Who);
            var online = ServerBase.Utils.GetActivePlayerCount() - 1;
            
            var embed = new EmbedBuilder()
            {
                Title=$"{Config.ServerName}",
                Description=$":small_orange_diamond: **{player?.Name}** has left the server",
                Color = new Color(174,77,38),
                Fields = new List<EmbedFieldBuilder>()
                {
                    new()
                    {
                        Name="Players Online:",
                        Value = $"{online} / {ServerBase.Config.Settings.MaxSlots}"
                    }
                }
            }.Build();
            Channel?.SendMessageAsync(embed:embed);
            UpdateStatus();
        }

        private void UpdateStatus()
        {
            Client.SetCustomStatusAsync(
                $"{ServerBase.Utils.GetActivePlayerCount()} / {ServerBase.Config.Settings.MaxSlots} players online");
        }


        private void OnPlayerChat(PlayerChatEventArgs args)
        {
            if (Config.ShowChatMessages is false)
            {
                return;
            }

            SocketGuildUser? discordUser = null;
            if (args.Player.IsLoggedIn)
            {
                discordUser = UserAuthentication.GetDiscordUserFromAccountId(args.Player.Account.AccountId);
            }
            
            var embed = new EmbedBuilder
            {
                Description=$"{args.RawText}",
                // TODO: Make color in-game prefix-based
                Color = new Color(102,102,102),
                Author = new EmbedAuthorBuilder()
                {
                    Url = (discordUser is not null ? $"https://discord.com/users/{discordUser?.Id}" : null),
                    Name = $"{args.Player.GetPrefix()}" + (discordUser is not null ? discordUser.DisplayName : args.Player.Name),
                    IconUrl = discordUser?.GetAvatarUrl() ?? "https://forums.terraria.org/data/avatars/h/0/50.jpg?1569505452"
                },
                Footer = new EmbedFooterBuilder
                {
                    Text = $"{Config.ServerName}"  
                }
            }.Build();
            Channel?.SendMessageAsync(embed:embed);
        }

        private void OnServerLoaded(EventArgs args)
        {
            Client = new DiscordSocketClient(new DiscordSocketConfig()
            {
                GatewayIntents = GatewayIntents.GuildMembers | GatewayIntents.Guilds | GatewayIntents.GuildMessages | GatewayIntents.DirectMessages
                
            });
            Client.Log += RelayLogger.DiscordLog;

            Task.Run(BotAsync);
        }

        private async Task BotAsync()
        {
            await Client.LoginAsync(TokenType.Bot, Config.Token);
            await Client.StartAsync();

            while (Client.ConnectionState != ConnectionState.Connected)
            {
                await Task.Delay(1000);
            }
            
            // set main channel
            Channel = await Client.GetChannelAsync(Config.ChannelId) as SocketTextChannel;
            
            if (Channel is null)
            {
                ServerBase.Log.ConsoleError($"Channel ID {Config.ChannelId} is invalid");
                this.Dispose();
                return;
            }
            
            // set log channel
            LogChannel = await Client.GetChannelAsync(Config.LogChannelId) as SocketTextChannel;

            if (LogChannel is null)
            {
                ServerBase.Log.ConsoleError($"Log channel ID {Config.ChannelId} is invalid, logging disabled.");
            }
            
            // will need to change this later, to support more types for ILog
            (ServerBase.Log as TextLog).WriteLine += (x) => LogChannel?.SendMessageAsync(x);
            
            CommandHandler commandHandler = new(Client, Channel);
            await commandHandler.InstallCommandsAsync();

            var embed = new EmbedBuilder()
            {
                Title=$"{Config.ServerName}",
                Description=":green_circle: Server has started!",
                Color = new Color(68,174,38)
            }.Build();

            await Channel.Guild.DownloadUsersAsync();
            Channel?.SendMessageAsync(embed:embed);
            UpdateStatus();

            await Task.Delay(-1);
        }
        
        protected override void Dispose(bool disposing)
        {
            var embed = new EmbedBuilder()
            {
                Title=$"{Config.ServerName}",
                Description=":red_circle: Server has stopped!",
                Color = new Color(237,127,84)
            }.Build();

            Channel?.SendMessageAsync(embed: embed);
        }
    }
}
