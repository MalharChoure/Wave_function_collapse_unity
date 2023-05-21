using cowsins; using UnityEngine; 
public class WeaponDefaultState : WeaponBaseState
{
    private WeaponController controller;

    private float holdProgress; 

    public WeaponDefaultState(WeaponStates currentContext, WeaponStateFactory playerStateFactory)
        : base(currentContext, playerStateFactory) { }

    public override void EnterState()
    {
        controller = _ctx.GetComponent<WeaponController>();

        holdProgress = 0;

        holdingEmpty = false; 
    }
        

    public override void UpdateState() {
        if (!_ctx.GetComponent<PlayerStats>().controllable) return;
        HandleInventory();
        if (InputManager.melee && controller.canMelee && controller.CanMelee) SwitchState(_factory.Melee());
        if (controller.weapon == null) return; 
        CheckSwitchState();
        CheckAim();
    }

    public override void FixedUpdateState() {}

    public override void ExitState() { }

    private float noBulletIndicator;

    private bool holdingEmpty = false; 
    public override void CheckSwitchState() {
        if (controller.canShoot && controller.id.bulletsLeftInMagazine > 0 && !controller.selectingWeapon 
            && (_ctx.GetComponent<PlayerMovement>().canShootWhileDashing && _ctx.GetComponent<PlayerMovement>().dashing || !_ctx.GetComponent<PlayerMovement>().dashing) )
        {
            switch (controller.weapon.shootMethod)
            {
                case ShootingMethod.Press:     
                    if (InputManager.shooting && !controller.holding)
                    {
                        controller.holding = true; // We are holding 
                        SwitchState(_factory.Shoot());
                    }
                    break;
                case ShootingMethod.PressAndHold:
                    if (InputManager.shooting) SwitchState(_factory.Shoot());
                    break;
                case ShootingMethod.HoldAndRelease:
                    if(!InputManager.shooting)
                    {
                        if(holdProgress > 100) SwitchState(_factory.Shoot());
                        holdProgress = 0;
                    }
                    if (InputManager.shooting)
                    {
                        Debug.Log(holdProgress);
                        holdProgress += Time.deltaTime * controller.weapon.holdProgressSpeed;
                        controller.holding = true; 
                    }
                    break;
                case ShootingMethod.HoldUntilReady:
                    if (!InputManager.shooting) holdProgress = 0; 
                    if (InputManager.shooting)
                    {
                        holdProgress += Time.deltaTime * controller.weapon.holdProgressSpeed; 
                        if (holdProgress > 100) SwitchState(_factory.Shoot());
                    }
                    break;
            }
        }

        if(controller.weapon.audioSFX.emptyMag != null)
        {
            if (controller.id.bulletsLeftInMagazine <= 0 && InputManager.shooting && noBulletIndicator <= 0 && !holdingEmpty)
            {
                SoundManager.Instance.PlaySound(controller.weapon.audioSFX.emptyMag, 0, .15f, 0);
                noBulletIndicator = (controller.weapon.shootMethod == ShootingMethod.HoldAndRelease || controller.weapon.shootMethod == ShootingMethod.HoldUntilReady) ? 1 : controller.weapon.fireRate;
                holdingEmpty = true; 
            }

            if (noBulletIndicator > 0) noBulletIndicator -= Time.deltaTime;

            if (!InputManager.shooting) holdingEmpty = false; 
        }

        if (controller.weapon.infiniteBullets) return; 
        if (InputManager.reloading && (int)controller.weapon.shootStyle != 2 && controller.id.bulletsLeftInMagazine < controller.weapon.magazineSize && controller.id.totalBullets > 0
            || controller.id.bulletsLeftInMagazine <= 0 && controller.autoReload && (int)controller.weapon.shootStyle != 2 && controller.id.bulletsLeftInMagazine < controller.weapon.magazineSize && controller.id.totalBullets > 0) SwitchState(_factory.Reload());
    }

    public override void InitializeSubState() { }

    private void CheckAim()
    {
        if (InputManager.aiming && _ctx.GetComponent<WeaponController>().weapon.allowAim) _ctx.GetComponent<WeaponController>().Aim();
        CheckStopAim(); 
    }

    private void CheckStopAim() {  if(!InputManager.aiming) _ctx.GetComponent<WeaponController>().StopAim(); }

    private void HandleInventory() => _ctx.GetComponent<WeaponController>().HandleInventory(); 
}
