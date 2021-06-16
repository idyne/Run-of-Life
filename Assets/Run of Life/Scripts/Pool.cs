using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FateGames;
public class Pool : MonoBehaviour
{
    [SerializeField] private float speed = 10;
    [SerializeField] private Renderer waterRend = null;
    public enum PoolType { GOOD, BAD };
    private static RunLevel levelManager = null;

    [SerializeField] private PoolType type = PoolType.GOOD;

    public PoolType Type { get => type; }
    public float Speed { get => speed; }

    private void Awake()
    {
        if (!levelManager)
            levelManager = (RunLevel)LevelManager.Instance;
        if (waterRend)
            waterRend.material.SetColor("_FoamColor", type == PoolType.GOOD ? levelManager.GoodPoolColor : levelManager.BadPoolColor);
    }

}
