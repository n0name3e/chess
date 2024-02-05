using System.Collections.Generic;
using UnityEngine;

public class Bishop
{
    private Piece pieceObject;
    private PieceDataList pieceData;

    private int[,] squareTable = new int[8, 8] // [x, y]
    {
        { 0, 0, 5, 0, 0, 5, 0, 0 },    // Rank 1 a
        { 0, 20, 0, 20, 10, 0, 20, 0 },    // Rank 2 b
        { 0, 0, 10, 15, 20, 10, 0, 0 },    // Rank 3 c
        { 0, 10, 15, 20, 20, 15, 10, 0 },    // Rank 4 d
        { 0, 10, 15, 20, 20, 15, 10, 0 },    // Rank 5 e
        { 0, 0, 10, 20, 15, 10, 0, 0 },    // Rank 6 f
        { 0, 10, 0, 10, 20, 0, 10, 0 },    // Rank 7 g
        { 0, 0, 5, 0, 0, 5, 0, 0 }     // Rank 8 h
    };

    public void Enable(Piece piece)
    {
        pieceData = JsonUtility.FromJson<PieceDataList>(Resources.Load<TextAsset>("json/pieceStats").text);

        this.pieceObject = piece;
        piece.GetMoves = CheckPossibleMoves;
        piece.GetSquareTable = GetTable;
        piece.maxHealth = Find().health;
        piece.maxMana = Find().mana;
        piece.healthRegeneration = Find().healthRegeneration;
        piece.manaRegeneration = Find().manaRegeneration;
        piece.cost = Find().cost;
    }
    private PieceData Find()
    {
        return pieceData.pieces.Find(x => x.piece == "Bishop");
    }

    private float GetTable(int x, int y)
    {
        if (pieceObject.color == Colors.White) return squareTable[x - 1, y - 1];
        else return squareTable[x - 1, 8 - y];
    }
    public List<Tile> CheckPossibleMoves(bool king = false, bool checkKing = false)
    {
        List<Tile> possibleTiles = new List<Tile>();
        for (int x = pieceObject.x + 1, y = pieceObject.y + 1; x <= 8 & y <= 8; x++, y++)
        {
            if (x > 8 || y > 8) break;
            Tile tile = pieceObject.board.FindTile(x, y);
            (bool addTile, bool canMove) = CheckMove(tile, king, checkKing);
            if (addTile == true) possibleTiles.Add(tile);
            if (canMove == false) break;
        }
        for (int x = pieceObject.x - 1, y = pieceObject.y + 1; x > 0 & y <= 8; x--, y++)
        {
            if (y > 8) break;
            Tile tile = pieceObject.board.FindTile(x, y);
            (bool addTile, bool canMove) = CheckMove(tile, king, checkKing);
            if (addTile == true) possibleTiles.Add(tile);
            if (canMove == false) break;
        }
        for (int x = pieceObject.x + 1, y = pieceObject.y - 1; x <= 8 & y > 0; x++, y--)
        {
            if (x > 8) break;
            Tile tile = pieceObject.board.FindTile(x, y);
            (bool addTile, bool canMove) = CheckMove(tile, king, checkKing);
            if (addTile == true) possibleTiles.Add(tile);
            if (canMove == false) break;
        }
        for (int x = pieceObject.x - 1, y = pieceObject.y - 1; x > 0 & y > 0; x--, y--)
        {
            Tile tile = pieceObject.board.FindTile(x, y);
            (bool addTile, bool canMove) = CheckMove(tile, king, checkKing);
            if (addTile == true) possibleTiles.Add(tile);
            if (canMove == false) break;
        }
        return possibleTiles;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="tile"></param>
    /// <returns>return false if cannot move further in this direction</returns>
    private (bool addTile, bool canMoveFurther) CheckMove(Tile tile, bool king = false, bool checkKing = false)
    {
        bool tiles = false; // true if should add move
        if (tile.CurrentPiece != null && tile.CurrentPiece.color == pieceObject.color)
        {
            if (king) tiles = true;
            return (tiles, false);
        }
        if (!king && checkKing)
        {
            bool a = GameManager.Instance.CreateVirtualBoard(pieceObject, tile, pieceObject.color);
            if (a == true)
            {
                if (tile.CurrentPiece != null && !king)
                {
                    return (tiles, false);
                }
                return (tiles, true);
            }
        }
        if (tile.CurrentPiece != null && tile.CurrentPiece.color != pieceObject.color)// && king)
        {
            tiles = true;
            if (king)
            {
                if (tile.CurrentPiece.pieceType == PieceType.King) return (tiles, true);
                return (tiles, false);
            }
            return (tiles, false);
            //return true;
        }
        if (tile.CurrentPiece == null)
        {
            tiles = true;
            return (tiles, true);
        }
        if (king && tile.CurrentPiece.color == pieceObject.color) tiles = true;

        return (false, false); // if ally piece
    }
}
