using RogueChess.Core;
using TMPro;
using UnityEngine;

namespace RogueChess.UI
{
    public class TurnUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text turnText;

        public void Refresh()
        {
            if (GameManager.Instance.IsGameOver)
            {
                turnText.text = $"{GameManager.Instance.WinningTeam} Wins!";
                return;
            }

            string turnString = $"{GameManager.Instance.CurrentTurn} Turn";
            
            if (GameManager.Instance.IsCurrentPlayerInCheck)
            {
                turnString += "\n<size=70%><color=#D05050>CHECK</color></size>";
            }

            turnText.text = turnString;
        }
    }
}