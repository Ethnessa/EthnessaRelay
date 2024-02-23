using MongoDB.Bson;

namespace EthnessaRelay.Database.Models;

public class UserAccountLink
{
    public ObjectId ObjectId { get; set; }
    
    public ulong DiscordId { get; set; }
    public int EthnessaId { get; set; }
}