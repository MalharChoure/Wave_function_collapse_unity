using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ragdoll : MonoBehaviour
{
    Rigidbody[] rigidbodies;
    Animator animator;

    // Start is called before the first frame update
    void Start()
    {
	    rigidbodies = GetComponentsInChildren<Rigidbody>();
        animator = GetComponent<Animator>();

        DeactivateRagdoll();
    }

    private void DeactivateRagdoll()
    {
	    foreach (var rigidbody in rigidbodies)
	    {
		    rigidbody.isKinematic = true;
	    }
        animator.enabled = true;
    }

    public void ActivateRagdoll()
    {
	    foreach (var rigidbody in rigidbodies)
	    {
		    rigidbody.isKinematic = false;
	    }
		animator.enabled = false;
	}
    // Update is called once per frame
    void Update()
    {
        
    }
}
