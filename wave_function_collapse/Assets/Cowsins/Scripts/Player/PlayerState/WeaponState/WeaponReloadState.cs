using cowsins; 
public class WeaponReloadState : WeaponBaseState
{

    public WeaponReloadState(WeaponStates currentContext, WeaponStateFactory playerStateFactory)
        : base(currentContext, playerStateFactory) { }

    public override void EnterState()
    {
        _ctx.GetComponent<WeaponController>().StartReload();
    }

    public override void UpdateState() {
        CheckSwitchState();
        if (!_ctx.GetComponent<PlayerStats>().controllable) return;
        CheckStopAim(); 
    }

    public override void FixedUpdateState()
    {
    }

    public override void ExitState() { }

    public override void CheckSwitchState() {
        if(!_ctx.GetComponent<WeaponController>().Reloading)SwitchState(_factory.Default());
    }

    public override void InitializeSubState() { }

    private void CheckStopAim()
    {
        if (!InputManager.aiming) _ctx.GetComponent<WeaponController>().StopAim();
    }

}
