using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FateGames;


public class Clock : MonoBehaviour
{
    private Color defaultBackColor;
    private Color defaulRimColor;
    private static RunLevel levelManager = null;
    private bool active = false;
    [SerializeField] private Renderer mesh = null;
    [SerializeField] private Transform hourHand = null;
    [SerializeField] private Transform minuteHand = null;

    private void Awake()
    {
        if (!levelManager)
            levelManager = (RunLevel)LevelManager.Instance;
        defaultBackColor = mesh.materials[1].color;
        defaulRimColor = mesh.materials[2].color;
    }
    void Update()
    {
        if (active)
        {

            hourHand.Rotate(new Vector3(0f, 0f, 1f), 6);
            minuteHand.Rotate(new Vector3(0f, 0f, 1f), 12);
        }
    }

    public void Mark()
    {
        mesh.materials[1].color = levelManager.ClockBackColor;
        mesh.materials[2].color = levelManager.ClockRimColor;
    }

    public void Unmark()
    {
        mesh.materials[1].color = defaultBackColor;
        mesh.materials[2].color = defaulRimColor;
    }


    public void ActivateClock()
    {
        active = true;
    }

    public void DeactivateClock()
    {
        active = false;
    }

    public void GoTransparent()
    {
        Renderer[] rends = GetComponentsInChildren<Renderer>();
        for (int i = 0; i < rends.Length; i++)
        {
            Material[] materials = rends[i].materials;
            for (int j = 0; j < materials.Length; j++)
            {
                Color newColor = materials[j].color;
                newColor.SetAlpha(0.2f);
                materials[j] = levelManager.TransparentMaterial;
                materials[j].color = newColor;
            }
            rends[i].materials = materials;
        }

    }

}
