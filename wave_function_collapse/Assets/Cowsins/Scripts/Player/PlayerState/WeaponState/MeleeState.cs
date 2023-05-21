using UnityEngine;
 
public class MeleeState : WeaponBaseState
{
    private float timer; 
    public MeleeState(WeaponStates currentContext, WeaponStateFactory playerStateFactory)
        : base(currentContext, playerStateFactory) { }

    public override void EnterState()
    {
        timer = 0;
       _ctx.GetComponent<WeaponController>().SecondaryMeleeAttack(); 
    }

    public override void UpdateState() {
        CheckSwitchState();
    }

    public override void FixedUpdateState()
    {
    }

    public override void ExitState() {
        _ctx.GetComponent<WeaponController>().FinishMelee(); 
    }

    public override void CheckSwitchState() {
        timer += Time.deltaTime;  

        if(timer >= _ctx.GetComponent<WeaponController>().meleeDuration + _ctx.GetComponent<WeaponController>().meleeDelay) SwitchState(_factory.Default());
    }

    public override void InitializeSubState() { }

}
