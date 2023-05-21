/// <summary>
/// This script belongs to cowsins™ as a part of the cowsins´ FPS Engine. All rights reserved. 
/// </summary>
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealMultiplierPowerUp : PowerUp
{
    [Header("CUSTOM"),SerializeField]
    private float healMultiplierAddition;

    public override void Interact(PlayerStats player)
    {
        base.Interact(player);
        player.healMultiplier += healMultiplierAddition;
        Destroy(this.gameObject);
    }
}
