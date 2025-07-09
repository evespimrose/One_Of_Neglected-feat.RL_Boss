using UnityEngine;

public class ArcherDashState : BaseState<Player>
{
    private float dashSpeed = 3f;
    private Vector2 dashDirection;
    private float dashDistance = 1f;

    private Vector2 startPosition;
    private Vector2 targetPosition;
    private float lerpProgress;

    private ParticleSystem currentDashEffect;

    public ArcherDashState(StateHandler<Player> handler) : base(handler) { }

    public override void Enter(Player player)
    {
        if (!player.CanDash())
        {
            handler.ChangeState(typeof(ArcherIdleState));
            return;
        }
        SoundManager.Instance.Play("Dash", SoundManager.Sound.Effect, 1.0f, false, 0.5f);
        player.ConsumeDash();
        player.SetDashing(true);
        player.SetSkillInProgress(true, false);

        player.InvokeDashDetect();

        player.Animator?.SetBool("IsMoving", false);
        player.Animator?.ResetTrigger("Idle");
        player.Animator?.ResetTrigger("Dash");
        player.Animator?.ResetTrigger("IsMoving");

        player.Animator?.Update(0);
        player.Animator?.SetTrigger("Dash");

        startPosition = player.transform.position;

        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");
        bool hasKeyboardInput = horizontalInput != 0 || verticalInput != 0;

        if (hasKeyboardInput)
        {
            dashDirection = new Vector2(horizontalInput, verticalInput).normalized;
        }
        else
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            dashDirection = (mousePosition - (Vector2)player.transform.position).normalized;
        }

        player.FlipModel(dashDirection.x < 0);

        if (player.DashEffect != null)
        {
            currentDashEffect = GameObject.Instantiate(player.DashEffect, player.transform.position, Quaternion.identity);
            currentDashEffect.transform.SetParent(player.transform);
            float angle = Mathf.Atan2(dashDirection.y, dashDirection.x) * Mathf.Rad2Deg;
            currentDashEffect.transform.rotation = Quaternion.Euler(0, dashDirection.x < 0 ? 180 : 0, 0);
            currentDashEffect.Play();
        }

        targetPosition = startPosition + (dashDirection * dashDistance);
        lerpProgress = 0f;
    }

    public override void Update(Player player)
    {
        lerpProgress += Time.deltaTime * dashSpeed;
        player.transform.position = Vector2.Lerp(startPosition, targetPosition, lerpProgress);

        if (lerpProgress >= 1f)
        {
            if (!player.IsAtDestination())
            {
                handler.ChangeState(typeof(ArcherMoveState));
            }
            else
            {
                handler.ChangeState(typeof(ArcherIdleState));
            }
        }
    }

    public override void Exit(Player player)
    {
        player.SetDashing(false);
        player.SetSkillInProgress(false, false);

        player.SetCurrentPositionAsTarget();
        player.InvokeDashCompleted();

        player.Animator?.ResetTrigger("Dash");
        player.Animator?.ResetTrigger("Idle");
        player.Animator?.ResetTrigger("IsMoving");

        player.Animator?.SetBool("IsMoving", false);
        player.Animator?.SetTrigger("Idle");

        if (currentDashEffect != null)
        {
            currentDashEffect.Stop();
            GameObject.Destroy(currentDashEffect.gameObject, 1f);
        }
    }
}
