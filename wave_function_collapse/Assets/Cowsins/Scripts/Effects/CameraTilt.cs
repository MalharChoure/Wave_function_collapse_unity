using UnityEngine;
using cowsins;

public class CameraTilt : MonoBehaviour
{
    [SerializeField] private PlayerMovement player;

    [SerializeField] private float tiltSpeed, tiltAmount;
    private void Update()
    {
        if (!PlayerStats.Controllable) return;
        Quaternion rot = Tilt(); 
        transform.localRotation = Quaternion.Lerp(transform.localRotation,rot,Time.deltaTime * tiltSpeed); 
    }

    private Quaternion Tilt()
    {
        if (player.currentSpeed == 0) return Quaternion.Euler(Vector3.zero);
        float x = InputManager.x;
        float y = InputManager.y; 

        Vector3 vector = new Vector3(y, 0, -x).normalized * tiltAmount;

        return Quaternion.Euler(vector); 
    }
}
