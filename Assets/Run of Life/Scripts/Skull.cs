using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skull : Collectible
{
    public override void GetCollected()
    {
        levelManager.Player.InstantDeath();
        ActivateEffect();
        DestroySelf();
    }

}
