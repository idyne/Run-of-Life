using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FateGames;
using TMPro;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Slider slider = null;
    [SerializeField] private Image fill = null;
    [SerializeField] private TextMeshProUGUI text = null;
    [SerializeField] private Gradient gradient = null;
    private RunLevel levelManager = null;

    private void Awake()
    {
        levelManager = (RunLevel)LevelManager.Instance;
    }

    public void UpdateBar()
    {
        slider.value = levelManager.Player.CurrentHealth / levelManager.Player.MaxHealth;
        fill.color = gradient.Evaluate(slider.value);

    }

    public void UpdateText()
    {
        text.text = levelManager.Player.CurrentAge.Name;
    }
}
