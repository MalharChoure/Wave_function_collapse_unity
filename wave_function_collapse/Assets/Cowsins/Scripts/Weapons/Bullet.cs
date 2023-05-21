/// <summary>
/// This script belongs to cowsins� as a part of the cowsins� FPS Engine. All rights reserved. 
/// </summary>
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [HideInInspector]public float speed,damage;

    [HideInInspector] public Vector3 destination;

    [HideInInspector] public bool gravity;

    [HideInInspector] public Transform player;

    [HideInInspector]public bool hurtsPlayer;

    [HideInInspector] public bool explosionOnHit;

    [HideInInspector] public GameObject explosionVFX;

    [HideInInspector] public float explosionRadius,explosionForce;

    [HideInInspector] public float criticalMultiplier;

    [HideInInspector] public float duration;

    private void Start()
    {
        transform.LookAt(destination);
        Invoke("DestroyProjectile", duration);
    }
    private void Update() => transform.Translate(0.0f, 0.0f, speed * Time.deltaTime);

    private bool hit = false;
    private void OnTriggerEnter(Collider other)
    {
        Vector3 contact = GetComponent<Collider>().ClosestPoint(transform.position);
        Vector3 direction = (transform.position - contact).normalized;

        if (!explosionOnHit)
        {
            if (other.CompareTag("Critical") && !hit)
            {
                other.transform.parent.GetComponent<IDamageable>().Damage(damage * criticalMultiplier);
                DestroyProjectile();
                hit = true;
                return;
            }
            if (other.GetComponent<IDamageable>() !=null && !hit && !other.CompareTag("Player"))
            {
                other.GetComponent<IDamageable>().Damage(damage);
                DestroyProjectile();
                hit = true;
                return;
            }        
        }
        else
        {
            Collider[] cols = Physics.OverlapSphere(transform.position, 10);
            foreach (Collider c in cols)
            {
                if (c.GetComponent<PlayerMovement>() != null) CamShake.instance.ExplosionShake(Vector3.Distance(CamShake.instance.gameObject.transform.position, transform.position));
            }
        }
        if (other.gameObject.layer == 3 || other.gameObject.layer == 8 || other.gameObject.layer == 10
            || other.gameObject.layer == 11 || other.gameObject.layer == 12 || other.gameObject.layer == 13 || other.gameObject.layer == 7) DestroyProjectile(); // Whenever it touches ground / object layer
    }


    private void DestroyProjectile()
    {
        if (explosionOnHit)
        {
            if (explosionVFX != null)
            {
                Vector3 contact = GetComponent<Collider>().ClosestPoint(transform.position);
                GameObject impact = Instantiate(explosionVFX, contact, Quaternion.identity);
                impact.transform.rotation = Quaternion.LookRotation(player.position);
            }
            Collider[] cols = Physics.OverlapSphere(transform.position, explosionRadius);

            foreach (Collider c in cols)
            {
                if (c.GetComponent<IDamageable>() != null)
                {
                    if (c.CompareTag("Player") && hurtsPlayer)
                    {
                        float dmg = damage * Mathf.Clamp01(1 - Vector3.Distance(c.transform.position, transform.position) / explosionRadius);
                        c.GetComponent<IDamageable>().Damage(dmg);
                    }
                    if (!c.CompareTag("Player"))
                    {
                        float dmg = damage * Mathf.Clamp01(1 - Vector3.Distance(c.transform.position, transform.position) / explosionRadius);
                        c.GetComponent<IDamageable>().Damage(dmg);
                    }
                }
                if (c.GetComponent<PlayerMovement>() != null)
                {
                    CamShake.instance.ExplosionShake(Vector3.Distance(CamShake.instance.gameObject.transform.position, transform.position));
                }
                if (c.GetComponent<Rigidbody>() != null)
                {
                   if(c != this) c.GetComponent<Rigidbody>().AddExplosionForce(explosionForce, transform.position, explosionRadius, 5, ForceMode.Force);
                }
            }
        }      
        Destroy(this.gameObject);
    }

    private void Destroy() => Destroy(this.gameObject);
}
