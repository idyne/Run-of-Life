using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace FateGames
{
    public class UICompleteScreen : UIElement
    {
        [HideInInspector] public bool Success = false;
        [SerializeField] private Text completeText = null;
        [SerializeField] private Text continueText = null;
        [SerializeField] private Text moneyText = null;
        [SerializeField] private GameObject moneyObject = null;

        public void SetScreen(bool success, int level)
        {
            completeText.text = GameManager.Instance.LevelName + " " + level + (success ? " COMPLETED" : " FAILED");
            continueText.text = success ? "CONTINUE" : "TRY AGAIN";
            if (success)
            {
                moneyObject.SetActive(true);
                moneyText.text = ((RunLevel)LevelManager.Instance).playerMoney.ToString();
            }
        }

        protected override void Animate()
        {
            return;
        }

        // Called by ContinueButton onClick
        public void Continue()
        {
            GameManager.Instance.LoadCurrentLevel();
        }

    }
}