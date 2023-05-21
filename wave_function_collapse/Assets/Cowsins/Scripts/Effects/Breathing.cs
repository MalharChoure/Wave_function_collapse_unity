/// <summary>
/// This script belongs to cowsins™ as a part of the cowsins´ FPS Engine. All rights reserved. 
/// </summary>
using System;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Attach this to a camera / camera holder in order to apply some cool breathing effect.
/// </summary>
public class Breathing : MonoBehaviour
{
    [Tooltip ("Attach your player with a rigidbody component"), SerializeField] private Rigidbody Player; 
    
    public float amplitude = .2f;

    [Tooltip("The higher the frequency is, the more it will waggle."), SerializeField] private float frequency = 2f;

    [Tooltip("Applies rotation effect when true"), SerializeField] private bool applyRotation; 

    private void Update()
    {
        // Apply some simple maths stuff to get a cool wave effect
        float angle = Time.timeSinceLevelLoad * frequency;
        float distance = amplitude * Mathf.Sin(angle)/ 400f;
        float distanceRot = amplitude * Mathf.Cos(angle) / 100f;

        // Apply this wave to the camera position
        transform.position = new Vector3(transform.position.x, transform.position.y + Vector3.up.y * distance, transform.position.z );

        // Also apply it to its rotation in case you want it
        if (!applyRotation) return;
        transform.Rotate(distanceRot, 0, 0, Space.Self);

    }

}
    
