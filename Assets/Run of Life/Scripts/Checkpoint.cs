using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour

{
    private bool activated = false;
    [SerializeField] private GameObject[] effectObjects = null;
    [SerializeField] private Transform[] flags = null;

    public bool Activated { get => activated; }
    public void Activate()
    {

        if (!activated)
        {
            activated = true;
            foreach (Transform flag in flags)
                flag.LeanRotateX(-90, 0.7f).setEaseOutElastic();
            foreach (GameObject effectObject in effectObjects)
                effectObject.SetActive(true);
        }

    }
}
