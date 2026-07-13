using RogueChess.Board;
using RogueChess.Core.Rules;
using RogueChess.Pieces;
using RogueChess.UI;
using UnityEngine;

namespace RogueChess.Core
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public Team CurrentTurn { get; private set; }
        public bool IsGameOver { get; private set; }

        public Team? WinningTeam { get; private set; }

        public bool IsCurrentPlayerInCheck => ChessRules.IsKingInCheck(CurrentTurn, FindAnyObjectByType<BoardManager>());

        [SerializeField] private CheckIndicatorManager checkIndicatorManager;
        [SerializeField] private TurnUI turnUI;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        private void Start()
        {
            StartGame();
        }

        public void StartGame()
        {
            CurrentTurn = Team.White;
            IsGameOver = false;
            WinningTeam = null;
            checkIndicatorManager.Refresh();
            turnUI.Refresh();
        }

        public void EndTurn()
        {
            CurrentTurn = CurrentTurn == Team.White ? Team.Black : Team.White;
            checkIndicatorManager.Refresh();
            turnUI.Refresh();
        }

        public void EndGame(Team winningTeam)
        {
            WinningTeam = winningTeam;
            IsGameOver = true;
            checkIndicatorManager.Refresh();
            turnUI.Refresh();
        }
    }
}