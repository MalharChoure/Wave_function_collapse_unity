using UnityEngine;
using cowsins; 
public class PlayerJumpState : PlayerBaseState
{
    public PlayerJumpState(PlayerStates currentContext, PlayerStateFactory playerStateFactory)
        : base(currentContext, playerStateFactory) { }

    PlayerMovement player;

    public override void EnterState() {
        player = _ctx.GetComponent<PlayerMovement>();
        _ctx.GetComponent<PlayerMovement>().events.OnJump.Invoke();
        _ctx.GetComponent<PlayerMovement>().Jump();
    }

    public override void UpdateState() {
        CheckSwitchState(); 
        HandleMovement();
        CheckUnCrouch(); 
    }

    public override void FixedUpdateState() { }

    public override void ExitState() {}

    public override void CheckSwitchState() {

        if (player.ReadyToJump && InputManager.jumping && (player.CanJump && player.grounded || player.wallRunning || player.jumpCount > 0 && player.maxJumps > 1 && player.CanJump))
            SwitchState(_factory.Jump());

        if (_ctx.GetComponent<PlayerStats>().health <= 0) SwitchState(_factory.Die());

        if (_ctx.GetComponent<PlayerMovement>().grounded || _ctx.GetComponent<PlayerMovement>().wallRunning )
           SwitchState(_factory.Default());
        if (player.canDash && InputManager.dashing && (player.infiniteDashes || player.currentDashes > 0 && !player.infiniteDashes)) SwitchState(_factory.Dash());

        if (InputManager.crouchingDown && !player.wallRunning)
        {
            if (player.allowCrouchWhileJumping) SwitchState(_factory.Crouch());
        }
    }

    public override void InitializeSubState() { }

    void HandleMovement()
    {
        _ctx.GetComponent<PlayerMovement>().Movement(_ctx.GetComponent<PlayerStats>().controllable);
        _ctx.GetComponent<PlayerMovement>().Look();
    }

    private bool canUnCrouch = false;

    private void CheckUnCrouch()
    {

        RaycastHit hitt;
        if (!InputManager.crouching) // Prevent from uncrouching when there´s a roof and we can get hit with it
        {
            if (Physics.Raycast(_ctx.transform.position, _ctx.transform.up, out hitt, 5.5f, _ctx.GetComponent<PlayerMovement>().weapon.hitLayer))
            {
                canUnCrouch = false;
            }
            else
                canUnCrouch = true;
        }
        if (canUnCrouch)
        {
            _ctx.GetComponent<PlayerMovement>().events.OnStopCrouch.Invoke(); // Invoke your own method on the moment you are standing up NOT WHILE YOU ARE NOT CROUCHING
            _ctx.GetComponent<PlayerMovement>().StopCrouch();
        }
    }
}
