using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FateGames;

public class Money : Collectible
{
    public override void GetCollected()
    {       
        ActivateEffect();      
        levelManager.playerMoney++;
        DestroySelf();
    }

}
