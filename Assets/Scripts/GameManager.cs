using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private Colors currentTurn = Colors.White;
    private bool gameEnded = false;
    private Delegates delegates;
    public bool showAbilities = true;
    public Colors CurrentTurn { get => currentTurn; set => currentTurn = value; }
    public bool GameEnded { get => gameEnded; private set => gameEnded = value; }
    public Delegates Delegates { get => delegates; private set => delegates = value; }


    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        Delegates = new Delegates();
    }
    public void EndTurn()
    {
        currentTurn = currentTurn == Colors.White ? Colors.Black : Colors.White;
        PawnPromotion();
        if (showAbilities)
        {
            foreach (Piece piece in BoardCreator.mainBoard.pieces)
            {
                if (piece.color != currentTurn)
                {
                    piece.Heal(piece.healthRegeneration);
                    piece.ReplenishMana(piece.manaRegeneration);
                    foreach (Ability ability in piece.abilities)
                    {
                        if (ability.cooldown > 0) ability.cooldown--;
                    }
                }
            }

            BuffManager.Instance.UpdateAllBuff(currentTurn);

            if (TileSelector.Instance.selectedPiece != null)
            {
                StartCoroutine(DisplayInfo());
            }
        }
        
        if (CurrentTurn != BoardCreator.playerColor)
        {
            StartCoroutine(MakeAiMove());
        }
        (bool c, Piece p) = KingInCheck(BoardCreator.mainBoard, currentTurn);
        if (c)
        {
            if (delegates.OnCheck != null) delegates.OnCheck.Invoke(p, currentTurn);
        }
        foreach (Piece pieces in BoardCreator.mainBoard.pieces)
        {
            if (pieces.color == Colors.Black) continue;
            if (pieces.pieceType == PieceType.King)
            {
                return;
            }
        }
        GameOver();
    }
    private IEnumerator MakeAiMove()
    {
        yield return null; // player will se his move
        ChessAI.Instance.MakeMove();
    }
    private IEnumerator DisplayInfo()
    {
        yield return null;
        UI.Instance.DisplayInfoContainer(TileSelector.Instance.selectedPiece, true);
    }
    public void StartGame()
    {
        GameEnded = false;
        BoardCreator.Instance.SpawnMainBoard();
        Abilities.Instance.a(BoardCreator.mainBoard);
    }
    public void GameOver()
    {
        GameEnded = true;
    }
    private void PawnPromotion()
    {
        /*foreach (Piece piece in BoardCreator.pieces) // pawn promotion
        {
            Pawn pawn = piece.pieceObject.GetComponent<Pawn>();
            if (pawn != null)
            {
                int lastRow = piece.color == Colors.White ? 8 : 1;
                if (piece.y == lastRow)
                {
                    Tile tile = BoardCreator.FindTile(piece.x, lastRow);
                    if (piece.color == Colors.White) tile.ChangeCurrentPiece(BoardCreator.queen);
                    else tile.ChangeCurrentPiece(BoardCreator.blackQueen);

                    break;
                }
            }
        }*/
    }

    /// <summary>
    /// check is king with colorToCheck color checked
    /// </summary>
    /// <param name="p">list of pieces used to check check</param>
    /// <param name="colorToCheck"></param>
    /// <returns></returns>
    public (bool check, Piece checker) KingInCheck(Board board, Colors colorToCheck)
    {
        //UnityEngine.Profiling.Profiler.BeginSample("Checking");
        List<Piece> pieces = board.pieces;
        Piece king = null;
        foreach(Piece piece in pieces)
        {
            if (piece.color != colorToCheck) continue;
            if (piece.pieceType == PieceType.King)
            {
                king = piece;
                break;
            }
        }
        if (king == null)
        {
            return (true, null); // king has been beaten 
        }
        foreach (Piece pie in pieces)
        {
            if (pie.color == colorToCheck) continue;
            List<Tile> tiles = pie.GetPossibleMoves(false, false);
            foreach (Tile move in tiles)
            {
                if (king.x == move.x && king.y == move.y)
                {                   
                    return (true, pie);
                }
            }
        }
        return (false, null);
    }

    /// <summary>
    /// to simulate move and check is it legal
    /// </summary>
    /// <param name="piece">piece to move</param>
    /// <param name="tile">tile to move piece</param>
    /// <returns> true if king will be checked after move </returns>
    public bool CreateVirtualBoard(Piece piece, Tile tile, Colors color)
    {
        Board cloneBoard = new Board();
        cloneBoard.CopyPieces(piece.board.pieces);

        Move move = new Move(cloneBoard.FindTile(piece.x, piece.y).CurrentPiece);

        move.MakeMove(cloneBoard.FindTile(tile.x, tile.y), cloneBoard);

        Piece king = null;
        foreach (Piece p in cloneBoard.pieces)
        {
            if (p.color != color) continue;
            if (p.pieceType == PieceType.King) king = p;
        }
        if (king == null) return true; // king has been beaten
        foreach (Piece p in cloneBoard.pieces)
        {
            if (p.color == color) continue;
            List<Tile> tiles = p.GetPossibleMoves(false, false);
            foreach (Tile t in tiles)
            {
                if (t.x == king.x && t.y == king.y) // checked
                {
                    cloneBoard.DestroyBoard();
                    return true;
                }
            }
        }
        cloneBoard.DestroyBoard();
        return false;
    }
    private void OnDestroy()
    {
        delegates.Clear();
    }
}
