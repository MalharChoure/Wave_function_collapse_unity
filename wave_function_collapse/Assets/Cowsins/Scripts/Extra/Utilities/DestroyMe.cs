/// <summary>
/// This script belongs to cowsins™ as a part of the cowsins´ FPS Engine. All rights reserved. 
/// </summary>
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyMe : MonoBehaviour
{
    public float timeToDestroy;

    void Start()
    {
        Invoke("DestroyMeObj", timeToDestroy);
    }

    // Update is called once per frame
    void DestroyMeObj()
    {
        Destroy(this.gameObject);
    }
}
