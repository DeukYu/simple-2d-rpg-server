namespace CS_Server;

public abstract class IJob
{
    public abstract void Execute();
    public bool Cancel { get; set; } = false;
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

    public override void Execute()
    {
        if (Cancel == false)
            _action.DynamicInvoke(_args);
    }
}