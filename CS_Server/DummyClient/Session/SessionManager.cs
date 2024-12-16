using ServerCore;

namespace DummyClient.Session;

public class SessionManager
{
    public static SessionManager Instance { get; } = new SessionManager();

    HashSet<ServerSession> _sessions = new HashSet<ServerSession>();
    object _lock = new object();
    int _dummyId = 1;

    public ServerSession Generate()
    {
        ServerSession session = new ServerSession();
        lock (_lock)
        {
            session.DummyId = _dummyId++;
            _sessions.Add(session);

            Log.Info($"Connected! {_sessions.Count} Players");
        }
        return session;
    }

    public void Remove(ServerSession session)
    {
        lock (_lock)
        {
            _sessions.Remove(session);
            Log.Info($"Disconnected! {_sessions.Count} Players");
        }
    }
}
