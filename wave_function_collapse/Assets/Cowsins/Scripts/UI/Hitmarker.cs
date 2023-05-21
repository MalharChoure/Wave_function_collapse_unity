using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cowsins; 

public class Hitmarker : MonoBehaviour
{

    public AudioClip crosshairSoundEffect; 
    private void Start() =>SoundManager.Instance.PlaySound(crosshairSoundEffect, .08f, .15f, 0);
}
