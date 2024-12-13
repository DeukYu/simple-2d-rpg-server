namespace CS_Server;

public abstract class JobBase
{
    public bool IsCanceled { get; set; } = false;
    public abstract void Execute();
}

public class Job : JobBase
{
    private readonly Delegate _action;
    private readonly object[] _args;

    public Job(Delegate action, params object[] args)
    {
        _action = action ?? throw new ArgumentNullException(nameof(action));
        _args = args ?? Array.Empty<object>();
    }

    public override void Execute()
    {
        if (IsCanceled == false)
            _action.DynamicInvoke(_args);
    }
}