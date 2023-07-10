using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class raycaster : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit; // Raycasthit is a struct that contains information about the raycast
        if (Physics.Raycast(transform.position, transform.forward, out hit)) // Raycast returns a bool, true if it hits something
        {
			Debug.Log(hit.transform.name);
		}
    }

    void OnDrawGizmos()
    {
		Debug.DrawLine(transform.position, transform.position + transform.forward * 100);
	}
}
