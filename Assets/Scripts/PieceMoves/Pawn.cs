using System.Collections.Generic;
using UnityEngine;

public class Pawn
{
    private Piece pieceObject;
    private PieceDataList pieceData;
    private Board board;

    private int[,] squareTable = new int[8, 8] // [y, x]
    {
        { 0, 0, 0, 0, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 0, 0, 0, 0 },
        { 5, 15, 20, 20, 20, 0, 15, 5 },
        { 0, -20, 30, 40, 40, 0, 0, 0 },
        { 0, -20, 30, 40, 40, 0, -20, 0 },
        { 0, 10, 20, 30, 30, 10, 10, 0 },
        { 0, 0, 0, 0, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 0, 0, 0, 0 }
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
        board = pieceObject.board;
    }
    private PieceData Find()
    {
        return pieceData.pieces.Find(x => x.piece == "Pawn");
    }
    private float GetTable(int x, int y)
    {
        if (pieceObject.color == Colors.White) return squareTable[y- 1, x - 1];
        else return squareTable[8 - y, x - 1];
    }
    private List<Tile> CheckPossibleMoves(bool king = false, bool checkKing = false)
    {
        List<Tile> possibleTiles = new List<Tile>();
        int x = pieceObject.x;
        int y = pieceObject.y;

        int forwardDirection = (pieceObject.color == Colors.White) ? 1: -1;
        int lastY = (pieceObject.color == Colors.White) ? 8: 1;
        int firstY = (pieceObject.color == Colors.White) ? 2: 7;
        if (king) // captures
        {
            // List<Tile> captureTiles = new List<Tile>();
            /*possibleTiles.Add(board.FindTile(x + 1, y + forwardDirection));
            possibleTiles.Add(board.FindTile(x - 1, y + forwardDirection));*/
            GetCaptureMoves(forwardDirection, possibleTiles);

            return possibleTiles;
            /*if (pieceObject.color == Colors.White)
            {
                if (x <= 7 && y <= 7)
                {
                    possibleTiles.Add(pieceObject.board.FindTile(x + 1, y + 1));
                }
                if (x >= 2 && y <= 7)
                {
                    possibleTiles.Add(pieceObject.board.FindTile(x - 1, y + 1));
                }
            }
            else if (pieceObject.color == Colors.Black)
            {
                if (x <= 7 && y >= 2)
                {
                    possibleTiles.Add(pieceObject.board.FindTile(x + 1, y - 1));
                }
                if (x >= 2 && y >= 2)
                {
                    possibleTiles.Add(pieceObject.board.FindTile(x - 1, y - 1));
                }
            }
            return possibleTiles;*/
        }

        Tile tile = board.FindTile(x, firstY + forwardDirection);
        if (CheckTile(tile)) possibleTiles.Add(tile);
        GetCaptureMoves(forwardDirection, possibleTiles); 
        /*if (tile.CurrentPiece == null)
        {
            if (checkKing) 
            {
                if (GameManager.instance.CreateVirtualBoard(pieceObject, tile, pieceObject.color) possibleTiles.Add(tile);
            }
            else possibleTiles.Add(tile);*/                
        }
        if (y == firstY) 
        {
            tile = board.FindTile(x, firstY + 2 * forwardDirection);
            if (CheckTile(tile)) possibleTiles.Add(tile);
        }
/*
        if (pieceObject.color == Colors.White)
        {
            Tile tile = pieceObject.board.FindTile(x, 3);
            if (y == 2)
            {
                if (tile.CurrentPiece == null)
                {
                    if (checkKing)
                    {
                        bool a = GameManager.instance.CreateVirtualBoard(pieceObject, tile, pieceObject.color);
                        if (!a) possibleTiles.Add(tile);
                    }
                    else
                    {
                        possibleTiles.Add(tile);
                    }

                    tile = pieceObject.board.FindTile(x, 4);
                    if (checkKing && tile.CurrentPiece == null)
                    {
                        if (!GameManager.instance.CreateVirtualBoard(pieceObject, tile, pieceObject.color)) possibleTiles.Add(tile);
                    }
                    else if (tile.CurrentPiece == null) possibleTiles.Add(tile);
                }
            }
            else if (y <= 7)
            {
                tile = pieceObject.board.FindTile(x, y + 1);
                if (tile.CurrentPiece == null)
                {
                    if (checkKing)
                    {
                        if (!GameManager.instance.CreateVirtualBoard(pieceObject, tile, pieceObject.color)) possibleTiles.Add(tile);
                    }
                    else possibleTiles.Add(tile);
                }
            }
            if (x <= 7 && y <= 7)
            {
                tile = pieceObject.board.FindTile(x + 1, y + 1);
                if (tile.CurrentPiece != null && tile.CurrentPiece.color != pieceObject.color)
                {
                    if (checkKing)
                    {
                        if (!GameManager.instance.CreateVirtualBoard(pieceObject, tile, pieceObject.color)) possibleTiles.Add(tile);
                    }
                    else possibleTiles.Add(tile);
                }
            }
            if (x >= 2 && y <= 7)
            {
                tile = pieceObject.board.FindTile(x - 1, y + 1);
                if (tile.CurrentPiece != null && tile.CurrentPiece.color != pieceObject.color)
                {
                    if (checkKing)
                    {
                        if (!GameManager.instance.CreateVirtualBoard(pieceObject, tile, pieceObject.color)) possibleTiles.Add(tile);
                    }
                    else possibleTiles.Add(tile);
                }
            }
        }
        if (pieceObject.color == Colors.Black)
        {
            Tile tile = pieceObject.board.FindTile(x, 6);
            if (y == 7)
            {
                if (tile.CurrentPiece == null)
                {
                    if (checkKing)
                    {
                        if (!GameManager.instance.CreateVirtualBoard(pieceObject, tile, pieceObject.color)) possibleTiles.Add(tile);
                    }
                    else possibleTiles.Add(tile);
                    tile = pieceObject.board.FindTile(x, 5);
                    if (tile.CurrentPiece == null)
                    {
                        if (checkKing)
                        {
                            if (!GameManager.instance.CreateVirtualBoard(pieceObject, tile, pieceObject.color)) possibleTiles.Add(tile);
                        }
                        else possibleTiles.Add(tile);
                    }
                }

            }
            else if (y >= 2)
            {
                tile = pieceObject.board.FindTile(x, y - 1);
                if (tile.CurrentPiece == null)
                {
                    if (checkKing)
                    {
                        if (!GameManager.instance.CreateVirtualBoard(pieceObject, tile, pieceObject.color)) possibleTiles.Add(tile);
                    }
                    else possibleTiles.Add(tile);
                }
            }
            if (x <= 7 && y >= 2)
            {
                tile = pieceObject.board.FindTile(x + 1, y - 1);
                if (tile.CurrentPiece != null && tile.CurrentPiece.color != pieceObject.color)
                {
                    if (checkKing)
                    {
                        if (!GameManager.instance.CreateVirtualBoard(pieceObject, tile, pieceObject.color)) possibleTiles.Add(tile);
                    }
                    else possibleTiles.Add(tile);
                }
            }
            if (x >= 2 && y >= 2)
            {
                tile = pieceObject.board.FindTile(x - 1, y - 1);
                if (tile.CurrentPiece != null && tile.CurrentPiece.color != pieceObject.color)
                {
                    if (checkKing)
                    {
                        if (!GameManager.instance.CreateVirtualBoard(pieceObject, tile, pieceObject.color)) possibleTiles.Add(tile);
                    }
                    else possibleTiles.Add(tile);
                }
            }
        }*/
        return possibleTiles;
    }
    private bool CheckTile(Tile tile, bool checkKing)
    {
        if (tile != null && tile.CurrentPiece == null)
        {
            if ((checkKing && GameManager.instance.CreateVirtualBoard(pieceObject, tile, pieceObject.color)) || !checkKing) return true;
        } 
        return false;
    }
    private void GetCaptureMoves(int forwardDirection, List<Tile> tiles)
    {
        possibleTiles.Add(board.FindTile(pieceObject.x + 1, pieceObject.y + forwardDirection));
        possibleTiles.Add(board.FindTile(pieceObject.x - 1, pieceObject.y + forwardDirection));
    }
}
