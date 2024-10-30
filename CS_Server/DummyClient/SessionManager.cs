namespace DummyClient;

class SessionManager
{
    static SessionManager _session = new SessionManager();
    public static SessionManager Instance { get { return _session; } }
    int _sessionId = 0;
    List<ServerSession> _sessions = new List<ServerSession>();
    object _lock = new object();

    public void SendForEach()
    {
        lock (_lock)
        {
            foreach (ServerSession session in _sessions)
            {
                C2S_Chat req = new C2S_Chat();
                req.chat = $"Hello Server!";
                ArraySegment<byte> segment = req.Write();

                session.Send(segment);
            }
        }
    }
        public ServerSession Generate()
    {
        lock (_lock)
        {
            ServerSession session = new ServerSession();
            _sessions.Add(session);

            return session;
        }
    }
}
