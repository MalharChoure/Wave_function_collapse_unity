using UnityEngine;
using System.Collections;
public class SoundManager :MonoBehaviour
{
    public static SoundManager Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            transform.parent = null; 
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(this.gameObject); 
    }

    public void PlaySound(AudioClip clip,float delay, float pitch,float spatialBlend)
    {
        StartCoroutine(Play(clip,delay,pitch,spatialBlend)); 
    }

    private IEnumerator Play(AudioClip clip, float delay, float pitch, float spatialBlend)
    {
        yield return new WaitForSeconds(delay);
        GetComponent<AudioSource>().spatialBlend = spatialBlend;
        GetComponent<AudioSource>().pitch = 1 + Random.Range(-pitch, pitch);
        GetComponent<AudioSource>().PlayOneShot(clip); 
        yield return null; 
    }
}
