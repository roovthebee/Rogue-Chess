using RogueChess.Core;
using TMPro;
using UnityEngine;

namespace RogueChess.UI
{
    public class TurnUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text turnText;

        private void Update()
        {
            if (GameManager.Instance.IsGameOver)
            {
                turnText.text = $"{GameManager.Instance.WinningTeam} Wins!";
            }
            else
            {
                turnText.text = $"{GameManager.Instance.CurrentTurn} Turn";
            }
        }
    }
}