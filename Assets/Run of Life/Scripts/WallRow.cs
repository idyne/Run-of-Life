using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FateGames;
using System.Linq;

public class WallRow : MonoBehaviour
{
    [SerializeField] private GameObject wallPrefab = null;
    [SerializeField] private int[] wallPoints = null;
    private bool deactivated = false;
    private static RunLevel levelManager = null;
    private List<Wall> walls = new List<Wall>();

    private void Awake()
    {
        if (!levelManager)
            levelManager = (RunLevel)LevelManager.Instance;
        Generate();
    }

    private void Generate()
    {
        Color[] colors = levelManager.WallColors;
        for (int i = 0; i < 4; i++)
        {
            if (wallPoints[i] > 0)
            {
                Wall wall = Instantiate(wallPrefab, transform.position + Vector3.right * (i - 1.5f), wallPrefab.transform.rotation, transform).GetComponent<Wall>();
                wall.MaxHealth = wallPoints[i];
                wall.ChangeColor(colors[Mathf.Clamp((int)(wall.MaxHealth / 10), 0, 4)]);
                wall.WallRow = this;
                walls.Add(wall);
            }
        }
    }

    public void Deactivate()
    {
        if (!deactivated)
        {
            deactivated = true;
            foreach (Wall wall in walls)
            {
                wall.GoTransparent();
            }
        }

    }
}
