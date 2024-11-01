namespace DummyClient;

class SessionManager
{
    static SessionManager _session = new SessionManager();
    public static SessionManager Instance { get { return _session; } }
    int _sessionId = 0;
    List<ServerSession> _sessions = new List<ServerSession>();
    object _lock = new object();
    Random _rand = new Random();

    public void SendForEach()
    {
        lock (_lock)
        {
            foreach (ServerSession session in _sessions)
            {
                C2S_Move movePacket = new C2S_Move();
                movePacket.posX = _rand.Next(-50, 50);
                movePacket.posY = 2;
                movePacket.posZ = _rand.Next(-50, 50);

                session.Send(movePacket.Write());
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
