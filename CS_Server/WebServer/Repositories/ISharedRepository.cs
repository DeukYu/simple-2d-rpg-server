using Microsoft.EntityFrameworkCore;
using Shared.DB;

namespace WebServer.Repositories;

public interface ISharedRepository
{
    Task<TokenInfo?> GetTokenByAccountId(long accountId);
    Task UpdateTokenAsync(TokenInfo tokenInfo, string token, DateTime expired);
    Task<TokenInfo> CreateTokenAsync(long accountId, string Token, DateTime expired);
    Task<List<ServerConfigInfo>> GetServerConfigInfosAsync();
    Task SaveChangesAsync();
}

public class SharedRepository : ISharedRepository
{
    private readonly SharedDB _context;
    public SharedRepository(SharedDB context)
    {
        _context = context;
    }

    public async Task<TokenInfo?> GetTokenByAccountId(long accountId)
    {
        return await _context.TokenInfo
            .FirstOrDefaultAsync(x => x.AccountId == accountId);
    }

    public async Task UpdateTokenAsync(TokenInfo tokenInfo, string token, DateTime expired)
    {
        tokenInfo.Token = token;
        tokenInfo.Expired = expired;
        await _context.SaveChangesExAsync();
    }

    public async Task<TokenInfo> CreateTokenAsync(long accountId, string token, DateTime expired)
    {
        var tokenInfo = new TokenInfo
        {
            AccountId = accountId,
            Token = token,
            Expired = expired
        };
        await _context.TokenInfo.AddAsync(tokenInfo);
        await _context.SaveChangesExAsync();
        return tokenInfo;
    }

    public async Task<List<ServerConfigInfo>> GetServerConfigInfosAsync()
    {
        return await _context.ServerConfigInfo.ToListAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesExAsync();
    }
}