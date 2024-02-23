using EthnessaAPI;
using Newtonsoft.Json;

namespace EthnessaRelay.Configuration;

public class RelaySettings
{
    private static string path => Path.Combine(ServerBase.SavePath,"relay.json");
    public string Token { get; set; } = "add your token and channel id here";
    public ulong ChannelId { get; set; } = 000000;

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