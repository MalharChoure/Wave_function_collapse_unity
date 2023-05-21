using UnityEngine;
using cowsins;

public class WeaponPickeable : Pickeable
{
    [Tooltip("Which weapon are we grabbing")] public Weapon_SO weapon;

    [HideInInspector] public int currentBullets, totalBullets;

    public override void Start()
    {
        base.Start();
        if (dropped) return;
        GetVisuals();
        currentBullets = weapon.magazineSize;
        totalBullets = weapon.totalMagazines * weapon.magazineSize;
    }

    public override void Interact()
    {
        WeaponController inv = player.GetComponent<WeaponController>();

        if (CheckIfInventoryFull()) // Inventory is full. 
        {
            Weapon_SO oldWeapon = inv.weapon;
            int saveBulletsLeftInMagazine = inv.id.bulletsLeftInMagazine;
            int saveTotalBullets = inv.id.totalBullets;
            inv.ReleaseCurrentWeapon();

            // Instantiating the selected weapon
            var weaponPicked = Instantiate(weapon.weaponObject, inv.weaponHolder);
            weaponPicked.transform.localPosition = weapon.weaponObject.transform.localPosition;
            //Assign the weapon to the inventory
            inv.inventory[inv.currentWeapon] = weaponPicked;

            inv.weapon = weapon;
            //Since this slot is selected, let´s unholster it
            inv.UnHolster(inv.inventory[inv.currentWeapon].gameObject);
            // Set bullets
            inv.inventory[inv.currentWeapon].GetComponent<WeaponIdentification>().bulletsLeftInMagazine = currentBullets;
            inv.inventory[inv.currentWeapon].GetComponent<WeaponIdentification>().totalBullets = totalBullets;
            //UI
            inv.slots[inv.currentWeapon].weapon = weapon;
            inv.slots[inv.currentWeapon].GetImage();
            //Now, let´s set the new weapon graphics on the pickeable
            currentBullets = saveBulletsLeftInMagazine;
            totalBullets = saveTotalBullets;

#if UNITY_EDITOR
            inv.crosshair.GetComponent<CrosshairShape>().currentPreset = inv.weapon.crosshairPreset;
            CowsinsUtilities.ApplyPreset(inv.crosshair.GetComponent<CrosshairShape>().currentPreset, inv.crosshair.GetComponent<CrosshairShape>());
#endif

            weapon = oldWeapon;
            image.sprite = weapon.icon;
            Destroy(graphics.transform.GetChild(0).gameObject);
            GetVisuals();
        }
        else
        {
            // We have already picked the weapon up in CheckIfInventoryFull()
            Destroy(this.gameObject);
        }
    }

    private bool CheckIfInventoryFull()
    {
        int i = 0;
        WeaponController inv = player.GetComponent<WeaponController>();

        while (i < inv.inventorySize)
        {
            if (inv.inventory[i] == null) // Inventory is, indeed, not full, so there is room for a new weapon.
            {
                // Instantiating the selected weapon
                var weaponPicked = Instantiate(weapon.weaponObject, inv.weaponHolder);
                weaponPicked.transform.localPosition = weapon.weaponObject.transform.localPosition;
                //Assign the weapon to the inventory
                inv.inventory[i] = weaponPicked;
                //Since this slot is selected and it was empty, let´s unholster it
                if (inv.inventory[inv.currentWeapon] == inv.inventory[i])
                {
                    inv.inventory[i].gameObject.SetActive(true);
                    inv.weapon = weapon;
                    inv.UnHolster(weaponPicked.gameObject);
                }
                else inv.inventory[i].gameObject.SetActive(false);
                // Set bullets
                inv.inventory[i].GetComponent<WeaponIdentification>().bulletsLeftInMagazine = currentBullets;
                inv.inventory[i].GetComponent<WeaponIdentification>().totalBullets = totalBullets;
                //UI
                inv.slots[i].weapon = weapon;
                inv.slots[i].GetImage();
#if UNITY_EDITOR
                if (inv.weapon != null)
                {
                    inv.crosshair.GetComponent<CrosshairShape>().currentPreset = inv.weapon.crosshairPreset;
                    CowsinsUtilities.ApplyPreset(inv.crosshair.GetComponent<CrosshairShape>().currentPreset, inv.crosshair.GetComponent<CrosshairShape>());
                }
#endif
                // Don´t return true
                return false;
            }
            i++;
        }
        // Inventory is full, we´ll check what to do then
        return true;

    }

    public void Drop(WeaponController wcon, Transform orientation)
    {
        dropped = true;
        GetComponent<Rigidbody>().AddForce(orientation.forward * 4, ForceMode.Impulse);
        GetComponent<Rigidbody>().AddForce(orientation.up * -2, ForceMode.Impulse);
        float random = Random.Range(-1f, 1f);
        GetComponent<Rigidbody>().AddTorque(new Vector3(random, random, random) * 10);

        currentBullets = wcon.id.bulletsLeftInMagazine;
        totalBullets = wcon.id.totalBullets;
        weapon = wcon.weapon;
        GetVisuals();
    }
    public void GetVisuals()
    {
        // Get whatever we need to display
        interactText = weapon._name;
        image.sprite = weapon.icon;
        // Manage graphics
        Destroy(graphics.transform.GetChild(0).gameObject);
        Instantiate(weapon.pickUpGraphics, graphics);
    }
}
