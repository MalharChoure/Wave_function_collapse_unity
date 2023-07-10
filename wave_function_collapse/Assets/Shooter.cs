using cowsins;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.HID;

public class Shooter : MonoBehaviour
{
	public Weapon_SO weapon;
	public Transform[] firePoint; 
	private int bulletsPerFire;
	RaycastHit hit;
	[Tooltip("What objects should be hit")] public LayerMask hitLayer;
	public Effects effects;

	public bool canShoot;

	private void Start()
	{
		canShoot = true;
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
			// CustomShotMethod();
		}
	}

	private IEnumerator HandleShooting()
	{
		// Determine wether we are sending a raycast, aka hitscan weapon, we are spawning a projectile or melee attacking
		int style = (int)weapon.shootStyle;

		//Determine weapon class / style
		//Dual shooting will be introduced coming soon
		switch (style)
		{
			case 0: //hitscan
				int i = 0;
				while (i < bulletsPerFire)
				{
					HitscanShot();
					
					if (weapon.useProceduralShot) ProceduralShot.Instance.Shoot(weapon.proceduralShotPattern);

					if(GetComponentInParent<Animator>().GetFloat("Speed") < 0.01)
						GetComponentInParent<Animator>().Play("Fire_Rifle_1");
					else if(GetComponentInParent<Animator>().GetFloat("Speed") < 3)
						GetComponentInParent<Animator>().Play("Fire_Rifle_2");
					else
						GetComponentInParent<Animator>().Play("Fire_Rifle_4");

					if (weapon.timeBetweenShots != 0) SoundManager.Instance.PlaySound(weapon.audioSFX.firing, 0, weapon.pitchVariationFiringSFX, 0);

					// ProgressRecoil();

					yield return new WaitForSeconds(weapon.timeBetweenShots);
					i++;
				}
				yield break;
			case 2:
				// MeleeAttack(weapon.attackRange, weapon.damagePerHit);
				break;
		}
	}

	private void HitscanShot()
	{
		// events.OnShoot.Invoke();

		Transform hitObj;

		//This defines the first hit on the object
		Vector3 dir = CowsinsUtilities.GetSpreadDirection(weapon.spreadAmount, firePoint[0]);
		Ray ray = new Ray(firePoint[0].transform.position, dir);

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

	// public void MeleeAttack(float attackRange, float damage)
	// {
	// 	// events.OnShoot.Invoke();
	//
	// 	Collider[] col = Physics.OverlapSphere(transform.position + mainCamera.transform.parent.forward * attackRange / 2, attackRange, hitLayer);
	//
	// 	float dmg = damage * GetComponent<PlayerStats>().damageMultiplier;
	//
	// 	foreach (var c in col)
	// 	{
	// 		if (c.transform.GetComponent<IDamageable>() != null)
	// 		{
	// 			if (c.CompareTag("Critical")) c.transform.parent.GetComponent<IDamageable>().Damage(dmg);
	// 			else c.GetComponent<IDamageable>().Damage(dmg);
	// 		}
	//
	// 	}
	//
	// 	//VISUALS
	// 	Ray ray = new Ray(mainCamera.transform.position, mainCamera.transform.forward);
	// 	if (Physics.Raycast(ray, out hit, attackRange, hitLayer))
	// 	{
	// 		Hit(hit.collider.gameObject.layer, 0f, hit, false);
	// 	}
	// }

	private void Hit(LayerMask layer, float damage, RaycastHit h, bool damageTarget)
	{
		// events.OnHit.Invoke();
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

	private float GetDistanceDamageReduction(Transform target)
	{
		if (!weapon.applyDamageReductionBasedOnDistance) return 1;
		if (Vector3.Distance(target.position, transform.position) > weapon.minimumDistanceToApplyDamageReduction)
			return (weapon.minimumDistanceToApplyDamageReduction / Vector3.Distance(target.position, transform.position)) * weapon.damageReductionMultiplier;
		else return 1;
	}
}
