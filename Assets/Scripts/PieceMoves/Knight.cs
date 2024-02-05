using System.Collections.Generic;
using UnityEngine;

public class Knight
{
    private Piece pieceObject;
    private PieceDataList pieceData;

    private int[,] squareTable = new int[8, 8] // [y, x]
    {
        { -10, -5, -10, -5, -5, -10, -5, -10 }, // File a (Rank 1)
        { -5, 5, 10, 10, 10, 10, 5, -5 },     // File b (Rank 2)
        { -10, 10, 25, 20, 20, 25, 10, -10 }, // File c (Rank 3)
        { -5, 10, 20, 40, 40, 20, 10, -5 },   // File d (Rank 4)
        { -5, 10, 20, 40, 40, 20, 10, -5 },   // File e (Rank 5)
        { -10, 10, 25, 20, 20, 25, 10, -10 }, // File f (Rank 6)
        { -5, 5, 10, 10, 10, 10, 5, -5 },     // File g (Rank 7)
        { -10, -5, -10, -5, -5, -10, -5, -10 } // File h (Rank 8)
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
        return pieceData.pieces.Find(x => x.piece == "Knight");
    }

    private float GetTable(int x, int y)
    {
        if (pieceObject.color == Colors.White) return squareTable[y - 1, x - 1];
        else return squareTable[8 - y, x - 1];
    }
    private List<Tile> CheckPossibleMoves(bool king = false, bool checkKing = false)
    {
        List<Tile> possibleTiles = new List<Tile>();
        int x = pieceObject.x;
        int y = pieceObject.y;

        if (CheckMove(pieceObject.board.FindTile(x + 1, y + 2), king, checkKing) == true) possibleTiles.Add(pieceObject.board.FindTile(x + 1, y + 2));
        if (CheckMove(pieceObject.board.FindTile(x - 1, y + 2), king, checkKing) == true) possibleTiles.Add(pieceObject.board.FindTile(x - 1, y + 2));
        if (CheckMove(pieceObject.board.FindTile(x + 2, y + 1), king, checkKing) == true) possibleTiles.Add(pieceObject.board.FindTile(x + 2, y + 1));
        if (CheckMove(pieceObject.board.FindTile(x - 2, y + 1), king, checkKing) == true) possibleTiles.Add(pieceObject.board.FindTile(x - 2, y + 1));
        if (CheckMove(pieceObject.board.FindTile(x + 2, y - 1), king, checkKing) == true) possibleTiles.Add(pieceObject.board.FindTile(x + 2, y - 1));
        if (CheckMove(pieceObject.board.FindTile(x - 2, y - 1), king, checkKing) == true) possibleTiles.Add(pieceObject.board.FindTile(x - 2, y - 1));
        if (CheckMove(pieceObject.board.FindTile(x + 1, y - 2), king, checkKing) == true) possibleTiles.Add(pieceObject.board.FindTile(x + 1, y - 2));
        if (CheckMove(pieceObject.board.FindTile(x - 1, y - 2), king, checkKing) == true) possibleTiles.Add(pieceObject.board.FindTile(x - 1, y - 2));
        
        return possibleTiles;
    }
    private bool CheckMove(Tile tile, bool king = false, bool checkKing = false)
    {
        if (tile == null) return false;
        if (tile.CurrentPiece != null && tile.CurrentPiece.color == pieceObject.color)
        {
            if (king) return true;
            return false;
        }
        if (checkKing && GameManager.Instance.CreateVirtualBoard(pieceObject, tile, pieceObject.color))
        {
            return false;
        }
        return true;
    }
}
