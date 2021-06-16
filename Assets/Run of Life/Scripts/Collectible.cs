using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FateGames;
public abstract class Collectible : MonoBehaviour
{
    protected static RunLevel levelManager;
    [SerializeField] private GameObject effectPrefab = null;
    [SerializeField] private Transform mesh = null;
    [SerializeField] private bool animate = true;


    private void Awake()
    {
        levelManager = (RunLevel)LevelManager.Instance;
    }

    private void Update()
    {
        if (animate)
            Animate();
    }

    public abstract void GetCollected();

    protected void ActivateEffect()
    {
        Destroy(Instantiate(effectPrefab, transform.position + Vector3.up * 0.75f, effectPrefab.transform.rotation), 5);
    }

    protected void Animate()
    {
        mesh.transform.Rotate(Vector3.forward, Time.deltaTime * 30);
    }


    protected void DestroySelf()
    {
        Destroy(GetComponent<Collider>());
        mesh.LeanScale(Vector3.zero, 0.3f).setOnComplete(() => { Destroy(gameObject); });
    }
}
