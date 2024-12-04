using ServerCore;
using System.Collections.Concurrent;

namespace CS_Server;

class SessionManager
{
    private static readonly SessionManager _instance = new SessionManager();
    public static SessionManager Instance => _instance;

    private int _sessionId = 0;
    private readonly ConcurrentDictionary<int, ClientSession> _sessions = new ConcurrentDictionary<int, ClientSession>();

    public ClientSession Generate()
    {
        int sessionId = Interlocked.Increment(ref _sessionId);
        var session = new ClientSession
        {
            SessionId = sessionId
        };

        if (_sessions.TryAdd(sessionId, session) == false)
        {
            Log.Error($"Failed to add session. SessionId({sessionId})");
            return null;
        }

        Log.Info($"Session generated. SessionId: {sessionId}");
        return session;
    }

    public ClientSession Find(int sessionId)
    {
        _sessions.TryGetValue(sessionId, out var session);
        return session;
    }
    public void Remove(ClientSession session)
    {
        if (session == null)
        {
            Log.Error("Session is null");
            return;
        }

        if (_sessions.TryRemove(session.SessionId, out var removedSession) == false)
        {
            Log.Error($"Failed to remove session. SessionId({session.SessionId})");
            return;
        }
    }
}
