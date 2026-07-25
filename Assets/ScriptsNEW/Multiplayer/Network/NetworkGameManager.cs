using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkGameManager : NetworkBehaviour, IMoveExecutor, ICardExecutor
{
    // Serialized Fields

    [SerializeField] private ChessRules chessRules;

    [SerializeField] private BoardManager boardManager;

    [SerializeField] private CardManager cardManager;

    [SerializeField] private GameManager gameManager;

    // Public Methods

    public void ExecuteMove(MoveData move)
    {
        if (IsHost)
        {
            ExecuteMoveInternal(move);

            BroadcastMoveClientRpc(new NetworkMove(move.StartCoordinate, move.TargetCoordinate));

            return;
        }

        RequestMoveServerRpc(new NetworkMove(move.StartCoordinate, move.TargetCoordinate));
    }

    public void ExecuteCard(Card card, CardTarget target)
    {
        NetworkCardPlay play = CreateNetworkCardPlay(card, target);

        if (IsHost)
        {
            if (ResolveCard(play))
            {
                FinishCardPlay(play);
            }
        }
        else
        {
            RequestPlayCardRpc(play);
        }
    }

    // Private Helpers

    private void ExecuteMoveInternal(MoveData move)
    {
        boardManager.ExecuteMove(move);

        GameManager.Instance.EndTurn();
    }

    private NetworkCardPlay CreateNetworkCardPlay(Card card, CardTarget target)
    {
        NetworkCardPlay play = new()
        {
            CardId = card.CardData.CardId,
            Owner = PlayerRoleManager.Instance.LocalTeam,
            EndsTurn = card.CardData.EndsTurn
        };

        if (target == null)
        {
            play.TargetType = TargetType.None;
        }
        else if (target is TileTarget tileTarget)
        {
            play.TargetType = TargetType.Tile;
            play.Coordinate = tileTarget.Coordinate;
        }
        else if (target is PieceTarget pieceTarget)
        {
            play.TargetType = TargetType.Piece;
            play.Coordinate = pieceTarget.Piece.Coordinate;
        }

        return play;
    }

    private CardTarget CreateCardTarget(NetworkCardPlay play)
    {
        switch (play.TargetType)
        {
            case TargetType.None:
                return null;

            case TargetType.Tile:
                return new TileTarget(play.Coordinate);

            case TargetType.Piece:

                Piece piece = boardManager.GetPiece(play.Coordinate);

                return piece == null ? null : new PieceTarget(piece);
            
            default:
                return null;
        }
    }

    private bool ResolveCard(NetworkCardPlay play)
    {
        Card card = cardManager.CreateCard(play.CardId);

        if (card == null)
        {
            return false;
        }

        CardTarget target = CreateCardTarget(play);

        CardContext context = new(card, target, play.Owner, boardManager, gameManager, cardManager);

        bool success = card.Resolve(context);

        if (!success)
        {
            return false;
        }

        if (IsHost)
        {
            BroadcastPlayCardRpc(play);
        }

        return true;
    }

    private void FinishCardPlay(NetworkCardPlay play)
    {
        cardManager.CompleteCardPlay();

        if (play.EndsTurn)
        {
            gameManager.EndTurn();
        }
    }

    // Rpcs

    [Rpc(SendTo.Server)]
    private void RequestMoveServerRpc(NetworkMove networkMove)
    {
        BoardCoordinate start = new(networkMove.StartX, networkMove.StartY);
        BoardCoordinate target = new(networkMove.TargetX, networkMove.TargetY);

        Piece piece = boardManager.GetPiece(start);

        if (piece == null)
        {
            return;
        }

        List<MoveData> legalMoves = chessRules.GetLegalMoves(piece);

        MoveData move = legalMoves.Find(m => m.TargetCoordinate == target);

        ExecuteMoveInternal(move);

        BroadcastMoveClientRpc(networkMove);
    }

    [Rpc(SendTo.NotServer)]
    private void BroadcastMoveClientRpc(NetworkMove networkMove)
    {
        if (IsHost)
        {
            return;
        }

        BoardCoordinate start = new(networkMove.StartX, networkMove.StartY);
        BoardCoordinate target = new(networkMove.TargetX, networkMove.TargetY);

        Piece piece = boardManager.GetPiece(start);

        if (piece == null)
        {
            return;
        }

        List<MoveData> legalMoves = chessRules.GetLegalMoves(piece);

        MoveData move = legalMoves.Find(m => m.TargetCoordinate == target);

        ExecuteMoveInternal(move);
    }

    [Rpc(SendTo.Server)]
    private void RequestPlayCardRpc(NetworkCardPlay play, RpcParams rpcParams = default)
    {
        if (ResolveCard(play))
        {
            FinishCardPlay(play);
        }
    }

    [Rpc(SendTo.NotServer)]
    private void BroadcastPlayCardRpc(NetworkCardPlay play)
    {
        if (ResolveCard(play))
        {
            FinishCardPlay(play);
        }
    }
}