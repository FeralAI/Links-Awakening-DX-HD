
namespace ProjectZ.InGame.GameObjects.Base.Components.AI;

class AiTriggerTimer(int startTime) : AiTrigger
{
    public int StartTime = startTime;
    public double CurrentTime;
    public bool State;

    public override void OnInit()
    {
        State = false;
        CurrentTime = StartTime;
    }
    
    public override void Update()
    {
        if (CurrentTime > 0)
            CurrentTime -= Game1.DeltaTime;

        if (CurrentTime <= 0)
            State = true;
    }

    public void Reset()
    {
        CurrentTime = StartTime;
        State = false;
    }
}
