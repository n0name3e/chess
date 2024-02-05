using System.Collections.Generic;
using UnityEngine;

public class Queen
{
    private Piece pieceObject;
    private PieceDataList pieceData;

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
        return pieceData.pieces.Find(x => x.piece == "Queen");
    }

    private float GetTable(int x, int y)
    {
        return 10;
    }
    public List<Tile> CheckPossibleMoves(bool king = false, bool checkKing = false)
    {
        List<Tile> possibleTiles = new List<Tile>();

        //bishop
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

        //rook
        for (int xPos = pieceObject.x + 1; xPos <= 8; xPos++)
        {
            if (xPos > 8) break;
            Tile tile = pieceObject.board.FindTile(xPos, pieceObject.y);
            (bool addTile, bool canMove) = CheckMove(tile, king, checkKing);
            if (addTile == true) possibleTiles.Add(tile);
            if (canMove == false) break;
        }
        for (int xPos = pieceObject.x - 1; xPos > 0; xPos--)
        {
            if (xPos < 1) break;
            Tile tile = pieceObject.board.FindTile(xPos, pieceObject.y);
            (bool addTile, bool canMove) = CheckMove(tile, king, checkKing);
            if (addTile == true) possibleTiles.Add(tile);
            if (canMove == false) break;
        }
        for (int yPos = pieceObject.y + 1; yPos <= 8; yPos++)
        {
            if (yPos > 8) break;
            Tile tile = pieceObject.board.FindTile(pieceObject.x, yPos);
            (bool addTile, bool canMove) = CheckMove(tile, king, checkKing);
            if (addTile == true) possibleTiles.Add(tile);
            if (canMove == false) break;
        }
        for (int yPos = pieceObject.y - 1; yPos > 0; yPos--)
        {
            if (yPos < 1) break;
            Tile tile = pieceObject.board.FindTile(pieceObject.x, yPos);
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
