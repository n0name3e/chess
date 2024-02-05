using System.Collections.Generic;
using UnityEngine;

public enum Colors
{
    White,
    Black
}
public enum PieceType
{
    Pawn,
    Knight,
    Bishop,
    Rook,
    Queen,
    King
}
public class Piece
{
    public PieceObject pieceObject;

    public Tile currentTile;
    public int x;
    public int y;
    public bool hasMoved = false;
    public Board board;
    public Colors color;

    public float maxHealth;
    public float maxMana;
    public float healthRegeneration;
    public float manaRegeneration;
    public int cost;

    public float health;
    public float mana;
    public int stunTimer { get; set; }

    public bool shouldMove = true;

    public List<Ability> abilities = new List<Ability>();
    public List<Buff> buffs = new List<Buff>();

    public PieceType pieceType;

    public delegate List<Tile> CheckMoves(bool king, bool checkKing);
    public CheckMoves GetMoves;

    public delegate float CheckTable(int x, int y);
    public CheckTable GetSquareTable;

    public delegate void Moving(Tile tile, Piece piece);
    public Moving OnMove;

    public void MovePiece(int xPos, int yPos, Board board = null)
    {
        if (board == null) board = BoardCreator.mainBoard;

        OnMove?.Invoke(board.FindTile(xPos, yPos), this);
        if (shouldMove)
        {
            currentTile.CurrentPiece = null;
            board.FindTile(xPos, yPos).ChangeCurrentPiece(this, board);
        }
    }
    public void DealDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            currentTile.CurrentPiece = null;
            currentTile = null;
            board.pieces.Remove(this);
            if (pieceObject != null) Object.Destroy(pieceObject.gameObject);
        }
    }
    public void Heal(float health)
    {
        this.health = Mathf.Clamp(this.health + health, 0, maxHealth);
    }
    public void ReplenishMana(float mana)
    {
        this.mana = Mathf.Clamp(this.mana + mana, 0, maxMana);
    }
    public bool isProtecredByPawn()
    {
        if (color == Colors.White)
        {
            Tile tile1 = board.FindTile(x - 1, y - 1);
            Tile tile2 = board.FindTile(x + 1, y - 1);
            if ((tile1 != null && tile1.CurrentPiece != null && tile1.CurrentPiece.color == Colors.White && tile1.CurrentPiece.pieceType == PieceType.Pawn)
              || tile2 != null && tile2.CurrentPiece != null && tile2.CurrentPiece.color == Colors.White && tile2.CurrentPiece.pieceType == PieceType.Pawn)
            {
                return true;
            }
        }
        if (color == Colors.Black)
        {
            Tile tile1 = board.FindTile(x - 1, y + 1);
            Tile tile2 = board.FindTile(x + 1, y + 1);
            if ((tile1 != null && tile1.CurrentPiece != null && tile1.CurrentPiece.color == Colors.White && tile1.CurrentPiece.pieceType == PieceType.Pawn)
              || tile2 != null && tile2.CurrentPiece != null && tile2.CurrentPiece.color == Colors.White && tile2.CurrentPiece.pieceType == PieceType.Pawn)
            {
                return true;
            }
        }
        return false;
    }
    public Ability FindAbility(string name)
    {
        foreach (Ability ability in abilities)
        {
            if (ability.name == name) return ability;
        }
        return null;
    }
    public void Stun(int duration)
    {
        stunTimer = Mathf.Max(stunTimer, duration);
        GameManager.Instance.Delegates.OnTurnEnd += ReduceStunTimer;
    }
    public bool IsStunned()
    {
        return stunTimer > 0;
    }
    public void ReduceStunTimer(Colors color)
    {
        if (color == this.color)
        {
            stunTimer--;
            if (stunTimer <= 0) GameManager.Instance.Delegates.OnTurnEnd -= ReduceStunTimer;
        }
    }
        public Piece(Piece clonePiece, Board board, bool changeTile = true)
    {
        x = clonePiece.x;
        y = clonePiece.y;
        this.board = board;
        cost = clonePiece.cost;
        color = clonePiece.color;
        GetMoves = clonePiece.GetMoves;
        GetSquareTable = clonePiece.GetSquareTable;
        pieceType = clonePiece.pieceType;
        if (changeTile) // hope i will remove this
        {
            board.FindTile(x, y).ChangeCurrentPiece(this, board);
        }

        InitializePieceType();
    }

    private void InitializePieceType()
    {
        if (pieceType == PieceType.Pawn) new Pawn().Enable(this);
        if (pieceType == PieceType.Knight) new Knight().Enable(this);
        if (pieceType == PieceType.Bishop) new Bishop().Enable(this);
        if (pieceType == PieceType.Rook) new Rook().Enable(this);
        if (pieceType == PieceType.Queen) new Queen().Enable(this);
        if (pieceType == PieceType.King) new King().Enable(this);

        health = maxHealth;
        mana = maxMana;
    }

    public Piece(PieceObject pieceObject)
    {
        this.pieceObject = pieceObject;
        pieceType = pieceObject.pieceType;
        InitializePieceType();
    }
    public List<Tile> GetPossibleMoves(bool king = false, bool checkKing = false)
    {
        if (GetMoves != null)
        {
            return GetMoves.Invoke(king, checkKing);
        }
        return null;
    }
    public void ClearDelegates()
    {
        GetMoves = null;
        GetSquareTable = null;
    }
}
