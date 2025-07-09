using UnityEngine;

public class WarriorDieState : BaseState<Player>
{
    private bool isDieAnimationPlayed = false;

    public WarriorDieState(StateHandler<Player> handler) : base(handler) { }

    public override void Enter(Player player)
    {
        if (player.Animator != null)
        {
            player.Animator.ResetTrigger("Idle");
            player.Animator.ResetTrigger("Attack");
            player.Animator.ResetTrigger("Dash");
            player.Animator.ResetTrigger("IsMoving");
            
            player.Animator.SetTrigger("Die");
            isDieAnimationPlayed = true;
        }
    }

    public override void Update(Player player)
    {
    }

    public override void Exit(Player player)
    {
        if (player.Animator != null)
        {
            player.Animator.SetTrigger("Idle");
            player.Animator.ResetTrigger("Die");
        }
        isDieAnimationPlayed = false;
    }
}
