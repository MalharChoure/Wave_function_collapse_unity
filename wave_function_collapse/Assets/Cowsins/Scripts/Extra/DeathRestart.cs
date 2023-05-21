using UnityEngine;
using cowsins;
using UnityEngine.SceneManagement; 

public class DeathRestart : MonoBehaviour
{
    private void Update()
    {
        if (InputManager.reloading) SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); 
    }
}
