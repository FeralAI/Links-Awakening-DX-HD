using System.Collections.Generic;

namespace ProjectZ.InGame.GameObjects.Base.Components.AI;

class AiState
{
    public delegate void InitFunction();
    public delegate void UpdateFunction();

    public InitFunction Init;
    public UpdateFunction Update;

    public List<AiTrigger> Trigger = [];

    public AiState() { }

    public AiState(UpdateFunction update)
    {
        Update = update;
    }
}
