using UnityEngine;

public class CameraFOVManager : MonoBehaviour
{
    [SerializeField] private Rigidbody player;

    private float baseFOV;

    private Camera cam;

    private PlayerMovement movement;

    private WeaponController weapon; 

    private void Start()
    {
        cam = GetComponent<Camera>();
        movement = player.GetComponent<PlayerMovement>();
        weapon = player.GetComponent<WeaponController>();

        // Grab the initial field of view
        baseFOV = cam.fieldOfView; 
    }

    private void Update()
    {
        if (weapon.isAiming && weapon.weapon != null ) return; // Not applicable if we are aiming

        // Do the wallrun motion
        if(movement.wallRunning && movement.canWallRun) cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, movement.wallrunningFOV, Time.deltaTime * movement.fadeInFOVAmount);
        else
        {
            // Running motion
            if(movement.currentSpeed > movement.walkSpeed && player.velocity.magnitude > .2f ) cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, movement.runningFOV, Time.deltaTime * movement.fadeInFOVAmount);
            else cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, baseFOV, Time.deltaTime * movement.fadeOutFOVAmount); // if we are not wallrunning nor running, go back to normal
        }
    }
}

