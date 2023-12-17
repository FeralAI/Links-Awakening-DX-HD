namespace ProjectZ.InGame.GameObjects.Base.Components.AI;

class AiTriggerCountdown(int startTime, AiTriggerCountdown.TriggerFunction tickFunction, AiTriggerCountdown.TriggerEndFunction countdownEnd, bool initRunningState = true) : AiTrigger
{
    public delegate void TriggerFunction(double counter);
    public delegate void TriggerEndFunction();

    public TriggerFunction TickFunction = tickFunction;
    public TriggerEndFunction CountdownEnd = countdownEnd;

    public double CurrentTime;
    public int StartTime = startTime;
    public bool ResetAfterEnd;

    private readonly bool _initRunningState = initRunningState;
    private bool _isRunning;

    public override void OnInit()
    {
        CurrentTime = StartTime;
        _isRunning = _initRunningState;
    }

    public override void Update()
    {
        if (!_isRunning)
            return;

        CurrentTime -= Game1.DeltaTime;

        if (CurrentTime <= 0)
        {
            _isRunning = false;
            CountdownEnd?.Invoke();
            if (ResetAfterEnd)
                OnInit();
        }
        else
            TickFunction?.Invoke(CurrentTime);
    }

    public bool IsRunning()
    {
        return _isRunning;
    }

    public void Restart()
    {
        CurrentTime = StartTime;
        _isRunning = true;
    }

    public void Start()
    {
        _isRunning = true;
    }

    public void Stop()
    {
        _isRunning = false;
    }
}
