using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FateGames;

public class Wall : MonoBehaviour
{
    [HideInInspector] public float MaxHealth;
    [HideInInspector] public WallRow WallRow = null;
    [SerializeField] private Transform wallMeshParent = null;
    [SerializeField] private Transform wallMesh = null;
    [SerializeField] private GameObject wallDestructionEffect = null;
    [SerializeField] private Text text = null;
    private Clock clock = null;
    private float currentHealth = 0;
    private float speed = 10;
    private static RunLevel levelManager = null;
    public Clock Clock { get => clock; }
    public Transform WallMesh { get => wallMesh; }

    private void Awake()
    {
        clock = gameObject.GetComponentInChildren<Clock>();
        if (!levelManager)
            levelManager = (RunLevel)LevelManager.Instance;
    }

    private void Start()
    {
        currentHealth = MaxHealth;
        text.text = Mathf.Ceil(currentHealth).ToString();
    }

    public float WallDestruction()
    {
        float result = currentHealth;
        if (currentHealth > 0)
        {
            currentHealth = Mathf.Clamp(currentHealth - Time.deltaTime * speed, 0, MaxHealth);
            result = currentHealth - result;
            Vector3 scale = wallMeshParent.localScale;
            scale.y = Mathf.Clamp(currentHealth / MaxHealth, 0.1f, 1);
            Vector3 effectPos = wallDestructionEffect.transform.position;
            effectPos.y = wallMesh.transform.position.y - (3 * currentHealth / MaxHealth);
            wallDestructionEffect.transform.position = effectPos;
            wallMeshParent.localScale = scale;
            text.text = ((int)currentHealth).ToString();
            return result;
        }
        WallRow.Deactivate();
        return 0;
    }

    public void GoTransparent()
    {
        GetComponentInChildren<Canvas>().gameObject.SetActive(false);
        Renderer rend = wallMesh.GetComponent<Renderer>();
        Color newColor = rend.material.color;
        newColor.SetAlpha(0.2f);
        rend.material = levelManager.TransparentMaterial;
        rend.material.color = newColor;
        clock.GoTransparent();
        Destroy(GetComponent<Collider>());
    }

    public void DestroySelf()
    {
        Destroy(Instantiate(levelManager.ClockDestroyEffectPrefab, clock.transform.position, levelManager.ClockDestroyEffectPrefab.transform.rotation), 1);
        Destroy(gameObject);
    }

    public void ChangeColor(Color newColor)
    {
        wallMesh.GetComponent<Renderer>().material.color = newColor;
        ParticleSystem.MainModule main = wallDestructionEffect.GetComponent<ParticleSystem>().main;
        main.startColor = newColor;
    }
}
