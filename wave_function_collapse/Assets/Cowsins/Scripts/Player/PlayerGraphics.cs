using UnityEngine;

public class PlayerGraphics : MonoBehaviour
{
    [SerializeField] private Transform player; 
    private void Update() => transform.position = player.position;
}
