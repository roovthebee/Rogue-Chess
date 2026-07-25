using System.Threading.Tasks;
using Unity.Services.Multiplayer;

public class SessionService
{
    // Public Methods

    public async Task<ISession> CreateSessionAsync()
    {
        SessionOptions options = new SessionOptions
        {
            MaxPlayers = 2
        };

        options.WithRelayNetwork();

        return await MultiplayerService.Instance.CreateSessionAsync(options);
    }

    public async Task<ISession> JoinSessionAsync(string joinCode)
    {
        return await MultiplayerService.Instance.JoinSessionByCodeAsync(joinCode);
    }
}