using System.Collections.Generic;
using UnityEngine;

public class King
{
    private Piece pieceObject;
    private PieceDataList pieceData;

    public Tile queenSideCastlingTile = null;
    public Tile kingSideCastlingTile = null;
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
        return pieceData.pieces.Find(x => x.piece == "King");
    }

    private float GetTable(int x, int y)
    {
        return -10;
    }
    private List<Tile> CheckPossibleMoves(bool king = false, bool checkKing = false)
    {
        queenSideCastlingTile = null;
        kingSideCastlingTile = null;
        //if (!checkKing && !king) return new List<Tile>();
        
        List<Tile> possibleTiles = new List<Tile>();
        List<Tile> tiles = new List<Tile>();

        int x = pieceObject.x;
        int y = pieceObject.y;

        if (x <= 7)
        {
            if (y <= 7)
            {
                tiles.Add(pieceObject.board.FindTile(x + 1, y + 1));
            }
            tiles.Add(pieceObject.board.FindTile(x + 1, y));
            if (y >= 2)
            {
                tiles.Add(pieceObject.board.FindTile(x + 1, y - 1));
            }
        }
        if (y <= 7)
        {
            tiles.Add(pieceObject.board.FindTile(x, y + 1));
        }
        if (y >= 2)
        {
            tiles.Add(pieceObject.board.FindTile(x, y - 1));
        }
        if (x >= 2)
        {
            if (y <= 7)
            {
                tiles.Add(pieceObject.board.FindTile(x - 1, y + 1));
            }
            tiles.Add(pieceObject.board.FindTile(x - 1, y));
            if (y >= 2)
            {
                tiles.Add(pieceObject.board.FindTile(x - 1, y - 1));
            }
        }
        if (king)
        {
            possibleTiles.AddRange(tiles);
            /*foreach(Tile tile in tiles)
            {
                possibleTiles.Add(tile);
            }*/
            return possibleTiles;
        }
        /*if (pieceObject.color != GameManager.instance.CurrentTurn)
        {
            MonoBehaviour.print("2");
            return new List<Tile>(0);
        }*/
        List<Tile> movesToRemove = new List<Tile>();
        foreach (Piece p in pieceObject.board.pieces)
        {
            if (pieceObject.color == p.color || p == pieceObject) continue;
            List<Tile> moves = p.GetPossibleMoves(true);
            foreach (Tile tile in tiles)    // possible king moves
            {
                foreach(Tile move in moves)
                {
                    if (tile.x == move.x && tile.y == move.y)
                    {
                        movesToRemove.Add(tile);
                    }
                }
            }            
        }
        foreach(Tile tile in tiles)
        {
            foreach(Tile move in movesToRemove)
            {
                if (tile.x == move.x && tile.y == move.y)
                {
                    goto A;
                }
            }
            if (tile.CurrentPiece != null)
            {
                if (tile.CurrentPiece.color == pieceObject.color)
                {
                    continue;
                }
            }
            possibleTiles.Add(tile);

            A: continue;
        }
        if (!king)
        {
            /*CheckKingSideCastling();
            CheckQueenSideCastling();*/
        }
        if (kingSideCastlingTile != null) possibleTiles.Add(kingSideCastlingTile);
        if (queenSideCastlingTile != null) possibleTiles.Add(queenSideCastlingTile);
        return possibleTiles;
    }
    private void CheckKingSideCastling()
    {       
        /*if (pieceObject.hasMoved) return;
        Piece a = pieceObject.board.FindTile(8, pieceObject.y).CurrentPiece;
        if (a == null) return;
        if (a.pieceType != PieceType.Rook && a.hasMoved) return;
        if (pieceObject.board.FindTile(pieceObject.x + 1, pieceObject.y).CurrentPiece != null || pieceObject.board.FindTile(pieceObject.x + 2, pieceObject.y).CurrentPiece != null)
        {
            return;
        }
        if (GameManager.instance.CreateVirtualBoard(pieceObject, pieceObject.board.FindTile(pieceObject.x + 1, 1), Colors.White) || GameManager.instance.CreateVirtualBoard(pieceObject, pieceObject.board.FindTile(pieceObject.x + 2, 1), Colors.White))
        {
            return;
        }
        kingSideCastlingTile = pieceObject.board.FindTile(pieceObject.x + 2, pieceObject.y);     
    */
        }
    private void CheckQueenSideCastling()
    {
        /*if (pieceObject.hasMoved) return;
        Piece a = pieceObject.board.FindTile(1, 1).CurrentPiece;
        if (a == null || a.pieceType != PieceType.Rook || a.hasMoved) return;
        if (pieceObject.board.FindTile(pieceObject.x - 1, 1).CurrentPiece != null || pieceObject.board.FindTile(pieceObject.x - 2, 1).CurrentPiece != null || pieceObject.board.FindTile(pieceObject.x - 3, 1).CurrentPiece != null)
        {
            return;
        }
        if (GameManager.instance.CreateVirtualBoard(pieceObject, pieceObject.board.FindTile(pieceObject.x - 1, 1), Colors.White) || GameManager.instance.CreateVirtualBoard(pieceObject, pieceObject.board.FindTile(pieceObject.x - 2, 1), Colors.White))
        {
            return;
        }
        queenSideCastlingTile = pieceObject.board.FindTile(pieceObject.x - 2, 1);*/
    }
}
