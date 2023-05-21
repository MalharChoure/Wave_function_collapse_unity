using UnityEngine;
using cowsins; 
public class PlayerCrouchState : PlayerBaseState
{

    public PlayerCrouchState(PlayerStates currentContext, PlayerStateFactory playerStateFactory)
        : base(currentContext, playerStateFactory) { }

    PlayerMovement player; 
    public override void EnterState() {
        player = _ctx.GetComponent<PlayerMovement>(); 
        player.events.OnCrouch.Invoke();
        player.StartCrouch();

    }

    public override void UpdateState() {
        if (InputManager.crouching) _ctx.transform.localScale = Vector3.MoveTowards(_ctx.transform.localScale, _ctx.GetComponent<PlayerMovement>().crouchScale, Time.deltaTime * _ctx.GetComponent<PlayerMovement>().crouchTransitionSpeed * 1.5f);
        CheckSwitchState();
        player.Look();
    }
    public override void FixedUpdateState() {
        HandleMovement(); 
    }

    public override void ExitState() {}

    public override void CheckSwitchState() {

        if (player.ReadyToJump && InputManager.jumping && player.canJumpWhileCrouching && (player.CanJump && player.grounded || player.wallRunning || player.jumpCount > 0 && player.maxJumps > 1 && player.CanJump))
            SwitchState(_factory.Jump());

        if (_ctx.GetComponent<PlayerStats>().health <= 0) SwitchState(_factory.Die());

        if (player.canDash && InputManager.dashing && (player.infiniteDashes || player.currentDashes > 0 && !player.infiniteDashes)) SwitchState(_factory.Dash());


        CheckUnCrouch(); 

    }

    public override void InitializeSubState() { }

    private bool canUnCrouch = false;

    void HandleMovement()
    {     
        player.Movement(_ctx.GetComponent<PlayerStats>().controllable);
        player.FootSteps();
        player.HandleSpeedLines(); 
    }

    void CheckUnCrouch()
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
            if (_ctx.transform.localScale == _ctx.GetComponent<PlayerMovement>().PlayerScale)
                SwitchState(_factory.Default());
        }
    }

}
