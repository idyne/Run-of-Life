using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthCollectible : Collectible
{
    [SerializeField] private float health = 30;
    public override void GetCollected()
    {
        levelManager.Player.AddHealth(health);
        ActivateEffect();
        DestroySelf();
    }
}
