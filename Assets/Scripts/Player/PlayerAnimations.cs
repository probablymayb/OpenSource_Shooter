using UnityEngine;

/// <summary>
/// This class has methods for controlling the player's animations. The PlayerController class is the 
/// one that calls those methods.
/// </summary>
public class PlayerAnimations : AnimationHandler
{
    // --------------------------------------
    // ----- 2D Isometric Shooter Study -----
    // ----------- by Tadadosi --------------
    // --------------------------------------
    // ---- Support my work by following ----
    // ---- https://twitter.com/tadadosi ----
    // --------------------------------------

    [TextArea(2, 10)]
    public string notes = "This class has methods for controlling the player's animations. The PlayerController class is the " +
        "one that calls those methods.";

    public enum AnimName { Idle, WalkForward, WalkBackwards }

    [SerializeField] private Animator animator;

    public void SetAnimationSpeed(AnimName name, float value)
    {
        switch (name)
        {
            case AnimName.Idle:
                animator.SetFloat("IdleSpeed", value);
                break;

            case AnimName.WalkForward:
                animator.SetFloat("WalkForwardSpeed", value / 2.5f);
                break;

            case AnimName.WalkBackwards:
                animator.SetFloat("WalkBackwardsSpeed", value / 2.5f);
                break;
        }
    }

    /// <summary>
    /// Method to determine if the animation should be walking forward, walking backwards or Idle
    /// </summary>
    /// <param name="moveInput">Any Horizontal + Vertical inputs as a <see cref="Vector3"/></param>
    public void PlayMoveAnimationsByMoveInputAndLookDirection(Vector3 moveInput)
    {
        bool movingRight = moveInput.x > 0 || moveInput.y > 0;
        bool movingLeft = moveInput.x < 0 || moveInput.y < 0;

        bool lookingRight = (TadaInput.IsMouseActive && crosshairMouse.AimDirection.x > 0) ||
                            (!TadaInput.IsMouseActive && CrosshairJoystick.AimDirection.x > 0);
        bool lookingLeft = (TadaInput.IsMouseActive && crosshairMouse.AimDirection.x < 0) ||
                            (!TadaInput.IsMouseActive && CrosshairJoystick.AimDirection.x < 0);

        if (movingRight && lookingRight || movingLeft && lookingLeft)
        {
            PlayAnimationIfNotAlready("WalkForward");
        }
        else if (movingRight && lookingLeft || movingLeft && lookingRight)
        {
            PlayAnimationIfNotAlready("WalkBackwards");
        }
        else
        {
            PlayAnimationIfNotAlready("Idle");
        }
    }

    private void PlayAnimationIfNotAlready(string triggerName)
    {
        AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);

        if (!state.IsName(triggerName))
        {
            // 다른 트리거 리셋 (안정성 확보)
            animator.ResetTrigger("Idle");
            animator.ResetTrigger("WalkForward");
            animator.ResetTrigger("WalkBackwards");

            animator.SetTrigger(triggerName);
        }
    }

}
