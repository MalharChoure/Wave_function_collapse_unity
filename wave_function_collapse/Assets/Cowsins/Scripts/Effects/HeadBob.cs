/// <summary>
/// This script belongs to cowsins™ as a part of the cowsins´ FPS Engine. All rights reserved. 
/// </summary>
using UnityEngine;

public class HeadBob : MonoBehaviour
{
    [SerializeField] private Rigidbody Player; // Attach your player with a rigidbody component

    [SerializeField] private float headBobAmplitude = .2f;

    [SerializeField] private float headBobFrequency = 2f;

    
    private bool jumping;

    private Vector3 origPos;

    private Quaternion origRot;

    void Awake()
    {
        jumping = false;
        origPos = transform.localPosition;
        origRot = transform.localRotation; 
    }
    void Update()
    {
        if (!PlayerStats.Controllable) return;
        if (Player.velocity.magnitude < Player.GetComponent<PlayerMovement>().walkSpeed || jumping)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition,origPos,Time.deltaTime * 2f);
            transform.localRotation = Quaternion.Lerp(transform.localRotation, origRot, Time.deltaTime * 2f);
            return;
        }
        float angle = Time.timeSinceLevelLoad * headBobFrequency;
        float distanceY = headBobAmplitude * Mathf.Sin(angle)/ 400f;
        float distanceX = headBobAmplitude * Mathf.Cos(angle) / 100f;
        transform.position = new Vector3(transform.position.x, transform.position.y + Vector3.up.y * distanceY, transform.position.z );
        transform.Rotate(distanceX, 0, 0, Space.Self);
    }

}
   