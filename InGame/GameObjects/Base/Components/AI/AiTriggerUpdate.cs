
namespace ProjectZ.InGame.GameObjects.Base.Components.AI;

class AiTriggerUpdate(AiTriggerUpdate.UpdateFunction update) : AiTrigger
{
    public delegate void UpdateFunction();
    public UpdateFunction UpdateFun = update;

    public override void Update()
    {
        UpdateFun?.Invoke();
    }
}
