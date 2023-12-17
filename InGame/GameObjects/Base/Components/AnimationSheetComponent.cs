namespace ProjectZ.InGame.GameObjects.Base.Components;

class AnimationSheetComponent(SheetAnimator animator) : BaseAnimationComponent
{
    public SheetAnimator Animator = animator;

    public override void UpdateAnimation()
    {
        Animator.Update();
    }
}
