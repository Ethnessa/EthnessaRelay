using Discord.WebSocket;
using EthnessaAPI;
using EthnessaAPI.Database;
using EthnessaAPI.Database.Models;
using EthnessaRelay.Database.Models;
using MongoDB.Driver;

namespace EthnessaRelay.Database;

public static class UserAuthentication
{
    private static readonly string CollectionName = "relay_users";

    private static IMongoCollection<UserAccountLink> authentication =>
        ServerBase.GlobalDatabase.GetCollection<UserAccountLink>(CollectionName);

    public static async Task<bool> AttemptUserLink(ulong discordId, string username, string password)
    {
        var accountLink = await authentication.Find(x => x.DiscordId == discordId).FirstOrDefaultAsync();

        if (accountLink is not null)
        {
            return false;
        }

        var userAccount = UserAccountManager.GetUserAccountByName(username);
        if (userAccount is null)
        {
            return false;
        }
        
        if (!userAccount.VerifyPassword(password))
        {
            return false;
        }
        
        await authentication.InsertOneAsync(new UserAccountLink
        {
            DiscordId = discordId,
            EthnessaId = userAccount.AccountId
        });

        return true;
    }
    
    public static SocketGuildUser? GetDiscordUserFromAccountId(int ethnessaAccountId)
    {
        var accountLink = authentication.Find(x => x.EthnessaId == ethnessaAccountId).FirstOrDefault();
        if(accountLink is null)
        {
            return null;
        }
        
        var user = EthnessaRelay.Channel?.Guild.GetUser(accountLink.DiscordId);

        return user;
    }
    
    public static async Task<UserAccount?> GetUserAccountFromDiscord(ulong discordId)
    {
        var accountLink = await authentication.Find(x => x.DiscordId == discordId).FirstOrDefaultAsync();

        if (accountLink is null)
        {
            return null;
        }

        var ethnessaAccount = UserAccountManager.GetUserAccountById(accountLink.EthnessaId);
        if (ethnessaAccount is null)
        {
            return null;
        }
        
        return ethnessaAccount;
    }
}