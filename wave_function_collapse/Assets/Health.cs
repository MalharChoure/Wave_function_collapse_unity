using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : Enemy
{
    // public float maxHealth = 100f;
    // public float currentHealth = 100f;
    Ragdoll ragdoll;

    // Start is called before the first frame update
    void Start()
    {
        ragdoll = GetComponent<Ragdoll>();
        // currentHealth = maxHealth;/
    }

    [SerializeField] private float timeToRevive;

    private bool isDead = false;

    public override void Damage(float damage)
    {
	    if (isDead) return;
	    base.Damage(damage);
	    GetComponent<Animator>().Play("Target_Hit");
    }
    public override void Die()
    {
	    if (isDead) return;
	    isDead = true;
	    events.OnDeath.Invoke();

	    if (shieldSlider != null) shieldSlider.gameObject.SetActive(false);
	    if (healthSlider != null) healthSlider.gameObject.SetActive(false);

	    if (UI.GetComponent<UIController>().displayEvents)
	    {
		    UI.GetComponent<UIController>().AddKillfeed(name);
	    }

	    if (transform.parent.GetComponent<CompassElement>() != null) transform.parent.GetComponent<CompassElement>().Remove();

	    GetComponent<Animator>().Play("Target_Die");
    }

	// public void TakeDamage(float damage)
 //    {
	// 	currentHealth -= damage;
	// 	if (currentHealth <= 0)
	// 	{
	// 		die();
	// 	}
	// }

    private void die()
    {
        ragdoll.ActivateRagdoll();
    }

 //    public override void Damage(float damage)
 //    {
	// 	currentHealth -= damage;
	// 	if (currentHealth <= 0)
	// 	{
	// 		die();
	// 	}
	// }
}
