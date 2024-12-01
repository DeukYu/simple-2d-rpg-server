namespace CS_Server;

public interface IJob
{
    void Execute();
}

public class Job : IJob
{
    private readonly Delegate _action;
    private readonly object[] _args;

    public Job(Delegate action, object[] args)
    {
        _action = action;
        _args = args;
    }

    public void Execute()
    {
        _action.DynamicInvoke(_args);
    }
}