using EthnessaAPI;
using Newtonsoft.Json;

namespace EthnessaRelay.Configuration;

public class RelaySettings
{
    private static string path => Path.Combine(ServerBase.SavePath,"relay.json");
    public string Token { get; set; } = "add your token and channel id here";
    public string ServerName { get; set; } = "Ethnessa Lobby";
    public ulong ChannelId { get; set; } = 000000;
    public ulong LogChannelId { get; set; } = 000000;
    
    public bool ShowJoinMessages { get; set; } = true;
    public bool ShowLeaveMessages { get; set; } = true;
    public bool ShowDeathMessages { get; set; } = true;
    public bool ShowChatMessages { get; set; } = true;

    public static RelaySettings Instance = new();
    
    public static RelaySettings Load()
    {
        if (File.Exists(path))
        {
            Instance = JsonConvert.DeserializeObject<RelaySettings>(File.ReadAllText(path));
            return Instance;
        }
        else
        {
            return Save();
        }
    }

    public static RelaySettings Save()
    {
        // format indented for readability
        File.WriteAllText(path, JsonConvert.SerializeObject(Instance, Formatting.Indented));
        return Instance;
    }
}