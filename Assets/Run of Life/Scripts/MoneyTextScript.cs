using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MoneyTextScript : MonoBehaviour
{
    Text text;
    public static int moneyAmount;
    void Start()
    {
        text = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        text.text = moneyAmount.ToString();
    }
}
