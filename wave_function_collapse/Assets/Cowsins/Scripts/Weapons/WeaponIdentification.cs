/// <summary>
/// This script belongs to cowsins� as a part of the cowsins� FPS Engine. All rights reserved. 
/// </summary>
using UnityEngine;

/// <summary>
/// Attach this to your weapon object ( the one that goes in the weapon array of WeaponController )
/// </summary>
public class WeaponIdentification : MonoBehaviour
{
    public Weapon_SO weapon;

    [Tooltip("Every weapon, excluding melee, must have a firePoint, which is the point where the bullet comes from." +
        "Just make an empty object, call it firePoint for organization purposes and attach it here. ")]
    public Transform[] FirePoint;

    [HideInInspector] public int totalMagazines, magazineSize, bulletsLeftInMagazine, totalBullets; // Internal use

    private void Start()
    {
        totalMagazines = weapon.totalMagazines;
        magazineSize = weapon.magazineSize;
        GetComponentInChildren<Animator>().keepAnimatorStateOnDisable = true;
    }

}
