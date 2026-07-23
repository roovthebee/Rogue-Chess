using TMPro;
using UnityEngine;

public class GameUI : MonoBehaviour
{
    // Serialized Fields

    [SerializeField] private GameManager gameManager;

    [SerializeField] private TMP_Text turnText;

    [SerializeField] private GameObject gameResultPanel;

    [SerializeField] private TMP_Text resultTitle;

    [SerializeField] private TMP_Text resultSubtitle;

    [SerializeField] private TMP_Text player1Text;

    [SerializeField] private TMP_Text player2Text;

    // Unity Messages

    private void Start()
    {
        gameResultPanel.SetActive(false);

        Refresh();
    }

    private void Refresh()
    {
        UpdateTurnText();
    }

    private void OnEnable()
    {
        gameManager.TurnChanged += HandleTurnChanged;
        gameManager.GameEnded += HandleGameEnded;
    }

    private void OnDisable()
    {
        gameManager.TurnChanged -= HandleTurnChanged;
        gameManager.GameEnded -= HandleGameEnded;
    }

    // Private Event Handlers

    private void HandleTurnChanged(Team currentTurn)
    {
        UpdateTurnText();
    }

    private void HandleGameEnded(Team? winningTeam)
    {
        if (winningTeam == null)
        {
            resultTitle.text = "DRAW";
            resultSubtitle.text = "Stalemate";
        }
        else
        {
            resultTitle.text = "CHECKMATE";
            resultSubtitle.text = $"{winningTeam} Wins!";
        }

        player1Text.text = "White";
        player2Text.text = "Black";

        gameResultPanel.SetActive(true);
    }

    // Private Helpers

    private void UpdateTurnText()
    {
        turnText.text = $"{gameManager.CurrentTurn} to Move";
    }
}