
namespace ProjectZ.InGame.GameObjects.Base.Components.AI;

class AiTriggerRandomTime(AiTriggerRandomTime.TriggerFunction triggered, int minTime, int maxTime) : AiTrigger
{
    public delegate void TriggerFunction();

    public TriggerFunction Triggered = triggered;

    public double CurrentTime;

    public int MinTime = minTime;
    public int MaxTime = maxTime;

    public bool IsRunning = true;
    public bool ResetAfterEnd = true;

    public override void OnInit()
    {
        IsRunning = true;
        CurrentTime = Game1.RandomNumber.Next(MinTime, MaxTime);
    }

    public override void Update()
    {
        if (!IsRunning)
            return;

        CurrentTime -= Game1.DeltaTime;

        if (CurrentTime > 0)
            return;

        Triggered();

        if (ResetAfterEnd)
            OnInit();
        else
            IsRunning = false;
    }
}
