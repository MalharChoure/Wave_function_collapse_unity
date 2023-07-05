using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using Pathfinding.Ionic.Zlib;
using UnityEngine;


public class aiFollow : MonoBehaviour
{
	// public Transform playerTransform;
	private AIPath aiPath;
	private Animator animator;
	public float a; 

	// Start is called before the first frame update
	void Start()
    {
        animator = GetComponent<Animator>();
		aiPath = GetComponent<AIPath>();
    }

    // Update is called once per frame
    void Update()
    {
	    a = aiPath.velocity.magnitude;
		animator.SetFloat("Speed", aiPath.velocity.magnitude);
    }
}
