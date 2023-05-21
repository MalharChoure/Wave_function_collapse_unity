using cowsins; 
public class WeaponShootingState : WeaponBaseState
{

    public WeaponShootingState(WeaponStates currentContext, WeaponStateFactory playerStateFactory)
        : base(currentContext, playerStateFactory) { }

    public override void EnterState() {
    }

    public override void UpdateState() {
        if (_ctx.GetComponent<WeaponController>().weapon == null) return;
        CheckSwitchState();
        if (!_ctx.GetComponent<PlayerStats>().controllable) return;
        CheckAim();
    }

    public override void FixedUpdateState()
    {
    }

    public override void ExitState() {
        _ctx.GetComponent<WeaponController>().Shoot();
    }

    public override void CheckSwitchState() {
        SwitchState(_factory.Default());
        if (_ctx.GetComponent<WeaponController>().weapon.infiniteBullets) return;
        if (InputManager.reloading && _ctx.GetComponent<PlayerStats>().controllable) SwitchState(_factory.Reload()); 
    }

    public override void InitializeSubState() { }

    private void CheckAim()
    {
        if (InputManager.aiming && _ctx.GetComponent<WeaponController>().weapon.allowAim) _ctx.GetComponent<WeaponController>().Aim();
        CheckStopAim();
    }
    private void CheckStopAim()
    {
        if (!InputManager.aiming) _ctx.GetComponent<WeaponController>().StopAim();
    }
}
