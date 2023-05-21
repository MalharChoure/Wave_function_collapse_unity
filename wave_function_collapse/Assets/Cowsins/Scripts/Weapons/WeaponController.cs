/// <summary>
/// This script belongs to cowsins™ as a part of the cowsins´ FPS Engine. All rights reserved. 
/// </summary>
using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.Events;
using cowsins;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor.Presets;
#endif

#region others
[System.Serializable]
public class Events
{
    public UnityEvent OnShoot, OnReload, OnFinishReload, OnAim, OnAiming, OnStopAim, OnHit, OnInventorySlotChanged;
}
[System.Serializable]
public class Effects
{
    public GameObject grassImpact, metalImpact, mudImpact, woodImpact, enemyImpact;
}
[System.Serializable]
public class CustomShotMethods
{
    public Weapon_SO weapon;
    public UnityEvent OnShoot;
}
#endregion

public class WeaponController : MonoBehaviour
{
    //References
    [Tooltip("Attach your weapon scriptable objects here.")] public Weapon_SO[] weapons;

    [Tooltip("An array that includes all your initial weapons.")]public Weapon_SO[] initialWeapons;

    public WeaponIdentification[] inventory;

    public UISlot[] slots;

    public Weapon_SO weapon;

    [Tooltip("Attach your main camera")] public Camera mainCamera;

    [Tooltip("Attach your camera pivot object")] public Transform cameraPivot;

    private Transform[] firePoint;

    [Tooltip("Attach your weapon holder")] public Transform weaponHolder;

    public Crosshair crosshair;

    //Variables

    [Tooltip("max amount of weapons you can have")] public int inventorySize;

    [SerializeField, HideInInspector]
    public bool isAiming;

    private bool reloading;
    public bool Reloading { get { return reloading; } set { reloading = value; } }

    [Tooltip("If true you won´t have to press the reload button when you run out of bullets")] public bool autoReload;

    [Tooltip("If false, hold to aim, and release to stop aiming.")] public bool alternateAiming;

    [Tooltip("What objects should be hit")] public LayerMask hitLayer;

    [Tooltip("Do you want to resize your crosshair on shooting ? "), SerializeField] private bool resizeCrosshair;

    [Tooltip("Do not draw the crosshair when aiming a weapon")] public bool removeCrosshairOnAiming;

    public bool canMelee;

    public bool CanMelee;

    [SerializeField] private GameObject meleeObject;

    [SerializeField] private Animator holsterMotionObject;

    public float meleeDuration, meleeAttackDamage, meleeRange, meleeCamShakeAmount, meleeDelay, reEnableMeleeAfterAction;

    private float spread;


    //UI
    [Tooltip("Attach the appropriate UI here")] public TextMeshProUGUI bulletsUI, magazineUI, reloadUI, lowAmmoUI;

    [Tooltip("Display an icon of your current weapon")] public Image currentWeaponDisplay;

    [Tooltip(" Attach the CanvasGroup that contains the inventory")] public CanvasGroup inventoryContainer;

    // Effects
    public Effects effects;

    public Events events;

    [Tooltip("Used for weapons with custom shot method. Here, " +
        "you can attach your scriptable objects and assign the method you want to call on shoot. " +
        "Please only assign those scriptable objects that use custom shot methods, Otherwise it won´t work or you will run into issues.")]public CustomShotMethods[] customShot;

    public UnityEvent customMethod;

    // Internal Use
    private int bulletsPerFire;

    public bool canShoot;

    RaycastHit hit;

    public int currentWeapon;

    private AudioClips audioSFX;

    public WeaponIdentification id;

    private PlayerStats stats;


    public bool holding;

    private void Start()
    {
        // Initial settings
        stats = GetComponent<PlayerStats>();
        inventory = new WeaponIdentification[inventorySize];
        currentWeapon = 0;
        canShoot = true;
        mainCamera.fieldOfView = GetComponent<PlayerMovement>().normalFOV;
        CanMelee = true;
        CreateInventoryUI();

        GetInitialWeapons();
    }

    private void Update()
    {
        if (weapon != null) firePoint = inventory[currentWeapon].GetComponent<WeaponIdentification>().FirePoint;
        HandleUI();
        HandleAimingMotion();
        ManageWeaponMethodsInputs();
        HandleRecoil();
    }

    private void HandleInput()
    {
        // Handle aiming
        if (!InputManager.aiming) isAiming = false;
        // + stop aiming if needed 
        if (!isAiming && weaponHolder.localPosition != Vector3.zero) StopAim();
    }

    public void Aim()
    {
        if (!isAiming) events.OnAim.Invoke(); // Invoke your custom method on stop aiming

        isAiming = true;

        if (weapon.applyBulletSpread) spread = weapon.aimSpreadAmount;

        events.OnAiming.Invoke();
        Vector3 newPos = weapon.aimingPosition; // Get the weapon aimingPosition
        weaponHolder.localPosition = Vector3.Lerp(weaponHolder.transform.localPosition, newPos, weapon.aimingSpeed * Time.deltaTime);
        weaponHolder.localRotation = Quaternion.Lerp(weaponHolder.transform.localRotation, Quaternion.Euler(weapon.aimingRotation), weapon.aimingSpeed * Time.deltaTime);
    }

    public void StopAim()
    {
        if (weapon != null && weapon.applyBulletSpread) spread = weapon.spreadAmount;

        if (isAiming) events.OnStopAim.Invoke(); // Invoke your custom method on stop aiming

        isAiming = false;

        Vector3 newPos = Vector3.zero;
        // Change the position and FOV
        weaponHolder.localPosition = Vector3.Lerp(weaponHolder.transform.localPosition, newPos, aimingSpeed * Time.deltaTime);
        weaponHolder.localRotation = Quaternion.Lerp(weaponHolder.transform.localRotation, Quaternion.Euler(newPos), aimingSpeed * Time.deltaTime);
    }

    private float aimingSpeed;

    private void HandleAimingMotion()
    {
        aimingSpeed = (weapon != null) ? weapon.aimingSpeed : 2;
        if (isAiming && weapon != null) mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, weapon.aimingFOV, weapon.aimingSpeed * Time.deltaTime);
    }
    public void Shoot()
    {
        int shootstyle = (int)weapon.shootStyle;
        // Hitscan or projectile
        if (shootstyle == 0 || shootstyle == 1)
        {
            foreach (var p in firePoint)
            {
                canShoot = false; // since you have already shot, you will have to wait in order to being able to shoot again
                bulletsPerFire = weapon.bulletsPerFire;
                StartCoroutine(HandleShooting());
            }
            if (weapon.timeBetweenShots == 0) SoundManager.Instance.PlaySound(weapon.audioSFX.firing, 0, weapon.pitchVariationFiringSFX, 0);
            Invoke("CanShoot", weapon.fireRate);
        }
        else if (shootstyle == 2) //Melee
        {
            canShoot = false;
            StartCoroutine(HandleShooting());
            Invoke("CanShoot", weapon.attackRate);
        }
        else // Check if this is a custom method weapon
        {
            // If we want to use fire Rate
            if (!weapon.continuousFire)
            {
                canShoot = false;
                Invoke("CanShoot", weapon.fireRate);
            }

            // Continuous fire
            CustomShotMethod();
        }
    }
    private void SelectCustomShotMethod()
    {
        // Iterate through each item in the array
        for (int i = 0; i < customShot.Length; i++)
        {
            // Assign the on shoot event to the unity event to call it each time we fire
            if (customShot[i].weapon == weapon)
            {
                customMethod = customShot[i].OnShoot;
                return;
            }
        }

        Debug.LogError("Appropriate weapon scriptable object not found in the custom shot array (under the events tab). Please, configure the weapon scriptable object and the suitable method to fix this error");
    }
    private void CustomShotMethod() => customMethod?.Invoke();

    private IEnumerator HandleShooting()
    {
        /// Determine wether we are sending a raycast, aka hitscan weapon, we are spawning a projectile or melee attacking
        int style = (int)weapon.shootStyle;
        // Adding a layer of realism, bullet shells get instantiated and interact with the world
        // We should obviously first check if we really wanna do this
        if (weapon.showBulletShells && style != 2)
        {
            foreach (var p in firePoint)
            {
                var b = Instantiate(weapon.bulletGraphics, p.position, mainCamera.transform.rotation);
                // Adding random rotation to the instantiated bullet shells
                float torque = Random.Range(-15, 15);
                b.GetComponent<Rigidbody>().AddTorque(mainCamera.transform.right * torque, ForceMode.Impulse);
                b.GetComponent<Rigidbody>().AddForce(mainCamera.transform.right * 5, ForceMode.Impulse);
                b.GetComponent<Rigidbody>().AddForce(mainCamera.transform.up * 5, ForceMode.Impulse);
            }
        }
        // Rest the bullets that have just been shot
        if (!weapon.infiniteBullets)
            id.bulletsLeftInMagazine -= weapon.ammoCostPerFire;

        //Determine weapon class / style
        //Dual shooting will be introduced coming soon
        switch (style)
        {
            case 0: //hitscan
                int i = 0;
                while (i < bulletsPerFire)
                {
                    HitscanShot();

                    CamShake.instance.ShootShake(weapon.camShakeAmount);
                    if (weapon.useProceduralShot) ProceduralShot.Instance.Shoot(weapon.proceduralShotPattern);

                    // Determine if we want to add an effect for FOV
                    if (weapon.applyFOVEffectOnShooting)
                    {
                        if (isAiming) mainCamera.fieldOfView = mainCamera.fieldOfView - weapon.AimingFOVValueToSubstract;
                        else mainCamera.fieldOfView = mainCamera.fieldOfView - weapon.FOVValueToSubstract;
                    }
                    foreach (var p in firePoint)
                    {
                        if (weapon.muzzleVFX != null)
                            Instantiate(weapon.muzzleVFX, p.position, mainCamera.transform.rotation, mainCamera.transform); // VFX
                    }
                    StartCoroutine(CowsinsUtilities.PlayAnim("shooting", inventory[currentWeapon].GetComponentInChildren<Animator>()));
                    if (weapon.timeBetweenShots != 0) SoundManager.Instance.PlaySound(weapon.audioSFX.firing, 0, weapon.pitchVariationFiringSFX, 0);

                    ProgressRecoil();

                    yield return new WaitForSeconds(weapon.timeBetweenShots);
                    i++;
                }
                yield break;
            case 1: // projectile   
                i = 0;
                while (i < bulletsPerFire)
                {
                    ProjectileShot();

                    CamShake.instance.ShootShake(weapon.camShakeAmount);
                    if (weapon.useProceduralShot) ProceduralShot.Instance.Shoot(weapon.proceduralShotPattern);

                    // Determine if we want to add an effect for FOV
                    if (weapon.applyFOVEffectOnShooting)
                    {
                        if (isAiming) mainCamera.fieldOfView = mainCamera.fieldOfView - weapon.AimingFOVValueToSubstract;
                        else mainCamera.fieldOfView = mainCamera.fieldOfView - weapon.FOVValueToSubstract;
                    }
                    foreach (var p in firePoint)
                    {
                        if (weapon.muzzleVFX != null)
                            Instantiate(weapon.muzzleVFX, p.position, mainCamera.transform.rotation, mainCamera.transform); // VFX
                    }
                    StartCoroutine(CowsinsUtilities.PlayAnim("shooting", inventory[currentWeapon].GetComponentInChildren<Animator>()));
                    if (weapon.timeBetweenShots != 0) SoundManager.Instance.PlaySound(weapon.audioSFX.firing, 0, weapon.pitchVariationFiringSFX, 0);

                    ProgressRecoil();

                    yield return new WaitForSeconds(weapon.timeBetweenShots);
                    i++;
                }
                break;
            case 2:
                MeleeAttack(weapon.attackRange, weapon.damagePerHit);
                CamShake.instance.ShootShake(weapon.camShakeAmount);
                // Determine if we want to add an effect for FOV
                if (weapon.applyFOVEffectOnShooting) mainCamera.fieldOfView = mainCamera.fieldOfView - weapon.FOVValueToSubstract;
                StartCoroutine(CowsinsUtilities.PlayAnim("shooting", inventory[currentWeapon].GetComponentInChildren<Animator>()));
                break;
        }
    }
    /// <summary>
    /// Hitscan weapons send a raycast that IMMEDIATELY hits the target.
    /// That is why this shooting method is mostly used for pistols, snipers, rifles or SMGs
    /// </summary>
    private void HitscanShot()
    {
        events.OnShoot.Invoke();
        if (resizeCrosshair && crosshair != null) crosshair.Resize(weapon.crosshairResize * 100);

        Transform hitObj;

        //This defines the first hit on the object
        Vector3 dir = CowsinsUtilities.GetSpreadDirection(weapon.spreadAmount, mainCamera);
        Ray ray = new Ray(mainCamera.transform.position, dir);

        if (Physics.Raycast(ray, out hit, weapon.bulletRange, hitLayer))
        {
            float dmg = weapon.damagePerBullet * GetComponent<PlayerStats>().damageMultiplier;
            Hit(hit.collider.gameObject.layer, dmg, hit, true);
            hitObj = hit.collider.transform;

            //Handle Penetration
            Ray newRay = new Ray(hit.point, ray.direction);
            RaycastHit newHit;

            if (Physics.Raycast(newRay, out newHit, weapon.penetrationAmount, hitLayer))
            {
                if (hitObj != newHit.collider.transform)
                {
                    float dmg_ = weapon.damagePerBullet * GetComponent<PlayerStats>().damageMultiplier * weapon.penetrationDamageReduction;
                    Hit(newHit.collider.gameObject.layer, dmg_, newHit, true);
                }
            }
        }
    }
    /// <summary>
    /// projectile shooting spawns a projectile
    /// Add a rigidbody to your bullet gameObject to make a curved trajectory
    /// This method is pretty much always used for grenades, rocket lFaunchers and grenade launchers.
    /// </summary>
    private void ProjectileShot()
    {
        events.OnShoot.Invoke();
        if (resizeCrosshair && crosshair != null) crosshair.Resize(weapon.crosshairResize * 100);

        Ray ray = mainCamera.ViewportPointToRay(new Vector3(.5f, .5f, 0f));
        Vector3 destination = (Physics.Raycast(ray, out hit) && !hit.transform.CompareTag("Player")) ? destination = hit.point + CowsinsUtilities.GetSpreadDirection(weapon.spreadAmount, mainCamera) : destination = ray.GetPoint(50f) + CowsinsUtilities.GetSpreadDirection(weapon.spreadAmount, mainCamera);

        foreach (var p in firePoint)
        {
            Bullet bullet = Instantiate(weapon.projectile, p.position, p.transform.rotation) as Bullet;

            if (weapon.explosionOnHit) bullet.explosionVFX = weapon.explosionVFX;

            bullet.hurtsPlayer = weapon.hurtsPlayer;
            bullet.explosionOnHit = weapon.explosionOnHit;
            bullet.explosionRadius = weapon.explosionRadius;
            bullet.explosionForce = weapon.explosionForce;

            bullet.criticalMultiplier = weapon.criticalDamageMultiplier;
            bullet.destination = destination;
            bullet.player = this.transform;
            bullet.speed = weapon.speed;
            bullet.GetComponent<Rigidbody>().isKinematic = (!weapon.projectileUsesGravity) ? true : false;
            bullet.damage = weapon.damagePerBullet * GetComponent<PlayerStats>().damageMultiplier;
            bullet.duration = weapon.bulletDuration;
        }
    }
    /// <summary>
    /// Moreover, cowsins´ FPS ENGINE also supports melee attacking
    /// Use this for Swords, knives etc
    /// </summary>
    public void MeleeAttack(float attackRange, float damage)
    {
        events.OnShoot.Invoke();

        Collider[] col = Physics.OverlapSphere(transform.position + mainCamera.transform.parent.forward * attackRange / 2, attackRange, hitLayer);

        float dmg = damage * GetComponent<PlayerStats>().damageMultiplier;

        foreach (var c in col)
        {
            if (c.transform.GetComponent<IDamageable>() != null)
            {
                if (c.CompareTag("Critical")) c.transform.parent.GetComponent<IDamageable>().Damage(dmg);
                else c.GetComponent<IDamageable>().Damage(dmg);
            }

        }

        //VISUALS
        Ray ray = new Ray(mainCamera.transform.position, mainCamera.transform.forward);
        if (Physics.Raycast(ray, out hit, attackRange, hitLayer))
        {
            Hit(hit.collider.gameObject.layer, 0f, hit, false);
        }
    }
    public void SecondaryMeleeAttack()
    {
        CanMelee = false;
        StartCoroutine(CowsinsUtilities.PlayAnim("hit", holsterMotionObject.GetComponent<Animator>()));
        meleeObject.SetActive(true);
        Invoke("Melee", meleeDelay);
    }

    private void Melee()
    {
        MeleeAttack(meleeRange, meleeAttackDamage);
        CamShake.instance.ShootShake(meleeCamShakeAmount);
    }

    public void FinishMelee()
    {
        StartCoroutine(CowsinsUtilities.PlayAnim("finished", holsterMotionObject.GetComponent<Animator>()));
        meleeObject.SetActive(false);
        Invoke("ReEnableMelee", reEnableMeleeAfterAction);
    }

    private void ReEnableMelee() => CanMelee = true;

    /// <summary>
    /// If you landed a shot onto an enemy, a hit will occur
    /// This is where that is being handled
    /// </summary>
    private void Hit(LayerMask layer, float damage, RaycastHit h, bool damageTarget)
    {
        events.OnHit.Invoke();
        GameObject impact = null, impactBullet = null;

        switch (layer)
        {
            case 10:
                impact = Instantiate(effects.grassImpact, h.point, Quaternion.identity); // Grass
                impact.transform.rotation = Quaternion.LookRotation(h.normal);
                if (weapon != null)
                    impactBullet = Instantiate(weapon.bulletHoleImpact.grassImpact, h.point, Quaternion.identity);
                break;
            case 11:
                impact = Instantiate(effects.metalImpact, h.point, Quaternion.identity); // Metal
                impact.transform.rotation = Quaternion.LookRotation(h.normal);
                if (weapon != null) impactBullet = Instantiate(weapon.bulletHoleImpact.metalImpact, h.point, Quaternion.identity);
                break;
            case 12:
                impact = Instantiate(effects.mudImpact, h.point, Quaternion.identity); // Mud
                impact.transform.rotation = Quaternion.LookRotation(h.normal);
                if (weapon != null) impactBullet = Instantiate(weapon.bulletHoleImpact.mudImpact, h.point, Quaternion.identity);
                break;
            case 13:
                impact = Instantiate(effects.woodImpact, h.point, Quaternion.identity); // Wood
                impact.transform.rotation = Quaternion.LookRotation(h.normal);
                if (weapon != null) impactBullet = Instantiate(weapon.bulletHoleImpact.woodImpact, h.point, Quaternion.identity);
                break;
            case 7:
                impact = Instantiate(effects.enemyImpact, h.point, Quaternion.identity); // Enemy
                impact.transform.rotation = Quaternion.LookRotation(h.normal);
                if (weapon != null) impactBullet = Instantiate(weapon.bulletHoleImpact.enemyImpact, h.point, Quaternion.identity);
                break;
        }

        if (h.collider != null && impactBullet != null)
        {
            impactBullet.transform.rotation = Quaternion.LookRotation(h.normal);
            impactBullet.transform.SetParent(h.collider.transform);
        }

        // Apply damage
        if (!damageTarget) return;
        if (h.collider.gameObject.CompareTag("Critical")) h.collider.transform.parent.GetComponent<IDamageable>().Damage(damage * weapon.criticalDamageMultiplier * GetDistanceDamageReduction(h.collider.transform));
        else if (h.collider.GetComponent<IDamageable>() != null) h.collider.GetComponent<IDamageable>().Damage(damage * GetDistanceDamageReduction(h.collider.transform));
    }


    private void CanShoot() => canShoot = true;

    private void FinishedSelection() => selectingWeapon = false;

    public void StartReload() => StartCoroutine(Reload());

    /// <summary>
    /// Handle Reloading
    /// </summary>
    private IEnumerator Reload()
    {
        events.OnReload.Invoke();
        SoundManager.Instance.PlaySound(weapon.audioSFX.reload, .1f, 0, 0);
        reloading = true;
        yield return new WaitForSeconds(.001f);


        StartCoroutine(CowsinsUtilities.PlayAnim("reloading", inventory[currentWeapon].GetComponentInChildren<Animator>()));

        yield return new WaitForSeconds(weapon.reloadTime);

        events.OnFinishReload.Invoke();

        reloading = false;
        canShoot = true;

        if (!weapon.limitedMagazines) id.bulletsLeftInMagazine = weapon.magazineSize;
        else
        {
            if (id.totalBullets > weapon.magazineSize) // You can still reload a full magazine
            {
                id.totalBullets = id.totalBullets - (weapon.magazineSize - id.bulletsLeftInMagazine);
                id.bulletsLeftInMagazine = weapon.magazineSize;
            }
            else if (id.totalBullets == weapon.magazineSize) // You can only reload a single full magazine more
            {
                id.totalBullets = id.totalBullets - (weapon.magazineSize - id.bulletsLeftInMagazine);
                id.bulletsLeftInMagazine = weapon.magazineSize;
            }
            else if (id.totalBullets < weapon.magazineSize) // You cant reload a whole magazine
            {
                int bulletsLeft = id.bulletsLeftInMagazine;
                if (id.bulletsLeftInMagazine + id.totalBullets <= weapon.magazineSize)
                {
                    id.bulletsLeftInMagazine = id.bulletsLeftInMagazine + id.totalBullets;
                    if (id.totalBullets - (weapon.magazineSize - bulletsLeft) >= 0) id.totalBullets = id.totalBullets - (weapon.magazineSize - bulletsLeft);
                    else id.totalBullets = 0;
                }
                else
                {
                    int ToAdd = weapon.magazineSize - id.bulletsLeftInMagazine;
                    id.bulletsLeftInMagazine = id.bulletsLeftInMagazine + ToAdd;
                    if (id.totalBullets - ToAdd >= 0) id.totalBullets = id.totalBullets - ToAdd;
                    else id.totalBullets = 0;
                }
            }
        }
    }
#if UNITY_EDITOR
    public Preset crosshairPreset;
#endif
    /// <summary>
    /// Active your new weapon
    /// </summary>
    public void UnHolster(GameObject weaponObj)
    {
        canShoot = true;

        weaponObj.SetActive(true);
        id = weaponObj.GetComponent<WeaponIdentification>();

        weaponObj.GetComponentInChildren<Animator>().enabled = true;
        StartCoroutine(CowsinsUtilities.PlayAnim("unholster", inventory[currentWeapon].GetComponentInChildren<Animator>()));
        SoundManager.Instance.PlaySound(weapon.audioSFX.unholster, .1f, 0, 0);
        Invoke("FinishedSelection", .5f);

        if (weapon.shootStyle == ShootStyle.Custom) SelectCustomShotMethod();
        else customMethod = null;
    }
    private void HandleUI()
    {
        //Inventory
        if (InputManager.scrolling != 0 && !InputManager.reloading) inventoryContainer.alpha = 1;
        else if (inventoryContainer.alpha > 0) inventoryContainer.alpha -= Time.deltaTime;

        // If we dont own a weapon yet, do not continue
        if (weapon == null)
        {
            bulletsUI.gameObject.SetActive(false);
            magazineUI.gameObject.SetActive(false);
            currentWeaponDisplay.gameObject.SetActive(false);
            reloadUI.gameObject.SetActive(false);
            lowAmmoUI.gameObject.SetActive(false);
            return;
        }
        if (weapon.infiniteBullets)
        {
            bulletsUI.gameObject.SetActive(false);
            magazineUI.gameObject.SetActive(false);
        }
        else
        {
            bulletsUI.gameObject.SetActive(true);
            magazineUI.gameObject.SetActive(true);
        }
        currentWeaponDisplay.gameObject.SetActive(true);

        currentWeaponDisplay.sprite = weapon.icon;

        if (!weapon.infiniteBullets)
        {

            // Set different display settings for each shoot style 
            if ((int)weapon.shootStyle != 2)
            {
                if (weapon.limitedMagazines)
                {
                    bulletsUI.text = id.bulletsLeftInMagazine.ToString();
                    magazineUI.text = " / " + id.totalBullets;
                }
                else
                {
                    bulletsUI.text = id.bulletsLeftInMagazine.ToString();
                    magazineUI.text = " / " + weapon.magazineSize;
                }
            }
            else
            {
                bulletsUI.text = null;
                magazineUI.text = null;
            }



            if (id.bulletsLeftInMagazine == 0 && !autoReload && !weapon.infiniteBullets) reloadUI.gameObject.SetActive(true);
            else reloadUI.gameObject.SetActive(false);

            if (id.bulletsLeftInMagazine < weapon.magazineSize / 3.5f && id.bulletsLeftInMagazine > 0) lowAmmoUI.gameObject.SetActive(true);
            else lowAmmoUI.gameObject.SetActive(false);
        }
        //Crosshair Management
        // If we dont use a crosshair stop right here
        if (crosshair == null)
        {
            crosshair.SpotEnemy(false);
            return;
        }
        // Detect enemies on aiming
        RaycastHit hit_;
        if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out hit_, weapon.bulletRange) && hit_.transform.CompareTag("Enemy") || Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out hit_, weapon.bulletRange) && hit_.transform.CompareTag("Critical"))
            crosshair.SpotEnemy(true);
        else crosshair.SpotEnemy(false);
    }
    /// <summary>
    /// Procedurally generate the Inventory UI depending on your needs
    /// </summary>
    private void CreateInventoryUI()
    {
        // Adjust the inventory size 
        slots = new UISlot[inventorySize];
        int j = 0; // Control variable
        while (j < inventorySize)
        {
            // Load the slot, instantiate it and set it to the slots array
            var slot = Instantiate(Resources.Load("InventoryUISlot"), Vector3.zero, Quaternion.identity, inventoryContainer.transform) as GameObject;
            slot.GetComponent<UISlot>().id = j;
            slots[j] = slot.GetComponent<UISlot>();
            j++;
        }
    }

    /// <summary>
    /// Change you current slots, core of the inventory
    /// </summary>
    public void HandleInventory()
    {
        if (InputManager.reloading) return; // Do not change weapons while reloading
        // Change slot
        if (InputManager.scrolling > 0 || InputManager.previousweapon)
            if (currentWeapon < inventorySize - 1)
            {
                currentWeapon++;
                SelectWeapon();
            }
        if (InputManager.scrolling < 0 || InputManager.nextweapon)
            if (currentWeapon > 0)
            {
                currentWeapon--;
                SelectWeapon();
            }

    }

    [HideInInspector] public bool selectingWeapon;
    public void SelectWeapon()
    {
        canShoot = false;
        selectingWeapon = true;
        crosshair.SpotEnemy(false);
        events.OnInventorySlotChanged.Invoke(); // Invoke your custom method
        weapon = null;
        // Spawn the appropriate weapon in the inventory

        foreach (WeaponIdentification weapon_ in inventory)
        {
            if (weapon_ != null)
            {
                weapon_.gameObject.SetActive(false);
                weapon_.GetComponentInChildren<Animator>().enabled = false;
                if (weapon_ == inventory[currentWeapon])
                {
                    weapon = inventory[currentWeapon].weapon;

                    weapon_.GetComponentInChildren<Animator>().enabled = true;
                    UnHolster(weapon_.gameObject);

#if UNITY_EDITOR
                    crosshair.GetComponent<CrosshairShape>().currentPreset = weapon.crosshairPreset;
                    CowsinsUtilities.ApplyPreset(crosshair.GetComponent<CrosshairShape>().currentPreset, crosshair.GetComponent<CrosshairShape>());
#endif
                }
            }
        }

        // Handle the UI Animations
        foreach (UISlot slot in slots)
        {
            slot.transform.localScale = slot.initScale;
            slot.GetComponent<CanvasGroup>().alpha = .2f;
        }
        slots[currentWeapon].transform.localScale = slots[currentWeapon].transform.localScale * 1.2f;
        slots[currentWeapon].GetComponent<CanvasGroup>().alpha = 1;

    }

    private void GetInitialWeapons()
    {
        if (initialWeapons.Length == 0) return;

        int i = 0;
        while (i < initialWeapons.Length)
        {
            var weaponPicked = Instantiate(initialWeapons[i].weaponObject, weaponHolder);
            weaponPicked.transform.localPosition = initialWeapons[i].weaponObject.transform.localPosition;

            inventory[i] = weaponPicked;

            if (i == currentWeapon)
            {
                weapon = weapons[i];
                UnHolster(weaponPicked.gameObject);
            }
            else weaponPicked.gameObject.SetActive(false);
            weapon = initialWeapons[i];

            inventory[i].GetComponent<WeaponIdentification>().bulletsLeftInMagazine = initialWeapons[i].magazineSize;
            if (initialWeapons[i].limitedMagazines)
                inventory[i].GetComponent<WeaponIdentification>().totalBullets = initialWeapons[i].magazineSize * initialWeapons[i].totalMagazines;
            else inventory[i].GetComponent<WeaponIdentification>().totalBullets = initialWeapons[i].magazineSize;

            //UI
            slots[i].weapon = weapon;
            slots[i].GetImage();
#if UNITY_EDITOR
            crosshair.GetComponent<CrosshairShape>().currentPreset = weapon.crosshairPreset;
            CowsinsUtilities.ApplyPreset(crosshair.GetComponent<CrosshairShape>().currentPreset, crosshair.GetComponent<CrosshairShape>());
#endif
            i++;
        }
        weapon = initialWeapons[0];

        if (weapon.shootStyle == ShootStyle.Custom) SelectCustomShotMethod();
        else customMethod = null;
    }

    public void ReleaseCurrentWeapon()
    {
        Destroy(inventory[currentWeapon].gameObject);
        weapon = null;
        slots[currentWeapon].weapon = null;
    }
    private float GetDistanceDamageReduction(Transform target)
    {
        if (!weapon.applyDamageReductionBasedOnDistance) return 1;
        if (Vector3.Distance(target.position, transform.position) > weapon.minimumDistanceToApplyDamageReduction)
            return (weapon.minimumDistanceToApplyDamageReduction / Vector3.Distance(target.position, transform.position)) * weapon.damageReductionMultiplier;
        else return 1;
    }

    private void ManageWeaponMethodsInputs()
    {
        if (!InputManager.shooting) holding = false; // Making sure we are not holding}
    }

    private float evaluationProgress, evaluationProgressX;
    private void HandleRecoil()
    {
        if (weapon != null && !weapon.applyRecoil)
        {
            cameraPivot.localRotation = Quaternion.Lerp(cameraPivot.localRotation, Quaternion.Euler(Vector3.zero), 3 * Time.deltaTime);
            return;
        }

        // Going back to normal shooting; 
        float speed = (weapon == null) ? 10 : weapon.recoilRelaxSpeed * 3;
        if (!InputManager.shooting || reloading || !PlayerStats.Controllable)
        {
            cameraPivot.localRotation = Quaternion.Lerp(cameraPivot.localRotation, Quaternion.Euler(Vector3.zero), speed * Time.deltaTime);
            evaluationProgress = 0;
            evaluationProgressX = 0;
        }

        if (weapon == null || reloading || !PlayerStats.Controllable) return;

        if (InputManager.shooting)
        {
            float xamount = (weapon.applyDifferentRecoilOnAiming && isAiming) ? weapon.xRecoilAmountOnAiming : weapon.xRecoilAmount;
            float yamount = (weapon.applyDifferentRecoilOnAiming && isAiming) ? weapon.yRecoilAmountOnAiming : weapon.yRecoilAmount;

            cameraPivot.localRotation = Quaternion.Lerp(cameraPivot.localRotation, Quaternion.Euler(new Vector3(-weapon.recoilY.Evaluate(evaluationProgress) * yamount, -weapon.recoilX.Evaluate(evaluationProgressX) * xamount, 0)), 10 * Time.deltaTime);
        }
    }

    private void ProgressRecoil()
    {
        if (weapon.applyRecoil)
        {
            evaluationProgress += 1f / weapon.magazineSize;
            evaluationProgressX += 1f / weapon.magazineSize;
        }
    }
}




