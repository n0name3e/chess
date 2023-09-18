using UnityEngine;
public class Tile
{
    private Piece currentPiece;
    public TileObject tileObject;
    public Ability currentAbility;
    public int x;
    public int y;
    public Board board;
    public System.Collections.Generic.List<Buff> buffs;

    public Piece CurrentPiece { get => currentPiece; set => currentPiece = value; }


    public void ChangeCurrentPiece(Piece piece, Board board = null)
    {
        if (board == null) board = BoardCreator.mainBoard;
        if (piece.pieceObject != null)
        {
            piece.pieceObject.transform.position = new Vector3(x, 0.02f, y);
        }
        piece.currentTile = this;
        if (currentPiece != null)
        {
            foreach (Piece p in board.pieces)
            {
                if (p.x == currentPiece.x && p.y == currentPiece.y)
                {
                    board.pieces.Remove(p);
                    p.currentTile = null;
                    break;
                }
            }
            if (CurrentPiece.pieceObject != null)
            {
                GameManager.instance.Delegates.OnCapture?.Invoke(piece, currentPiece);

                Object.Destroy(currentPiece.pieceObject.gameObject);
            }
            AddAbilitiesOnCaptureCharges(board);
        }
        piece.x = x;
        piece.y = y;
        currentPiece = piece;
        piece.board = board;
    }

    private void AddAbilitiesOnCaptureCharges(Board board)
    {
        foreach (Piece p in board.pieces)
        {
            if (currentPiece.pieceType == PieceType.Pawn) break;
            if (p.color == currentPiece.color) continue;
            foreach (Ability a in p.abilities)
            {
                if (a.abilityData.GetChargeCondition("captureEnemyPiece"))
                {
                    a.charges++;
                }
            }
        }
    }

    public GameObject SpawnPiece(GameObject pieceObject, Board board = null)//, bool setTile = true)
    {
        if (board == null) board = BoardCreator.mainBoard;

        GameObject piece = Object.Instantiate(pieceObject, new Vector3(x, 0.02f, y), Quaternion.Euler(new Vector3(90, 0, 0)));
        Piece p = piece.GetComponent<PieceObject>().piece;
        p.board = board;
        p.x = x;
        p.y = y; 
        board.pieces.Add(p);
        ChangeCurrentPiece(p, board);
        return piece;
    }
    public bool isAttackedByPiece(Colors color)
    {
        foreach (Piece piece in board.pieces)
        {
            if (piece.color != color) continue;
            foreach (Tile tile in piece.GetPossibleMoves(true))
            {
                if (tile.x == x && tile.y == y)
                {
                    return true;
                }
            }
        }
        return false;
    }
    public System.Collections.Generic.List<Tile> GetNeighbourTiles()
    {        
        System.Collections.Generic.List<Tile> tiles = new System.Collections.Generic.List<Tile>(9);
        if (board.FindTile(x + 1, y + 1) != null) tiles.Add(board.FindTile(x + 1, y + 1));
        if (board.FindTile(x + 1, y - 1) != null) tiles.Add(board.FindTile(x + 1, y - 1));
        if (board.FindTile(x + 1, y) != null) tiles.Add(board.FindTile(x + 1, y));
        if (board.FindTile(x, y + 1) != null) tiles.Add(board.FindTile(x, y + 1));
        if (board.FindTile(x, y - 1) != null) tiles.Add(board.FindTile(x, y - 1));
        if (board.FindTile(x - 1, y + 1) != null) tiles.Add(board.FindTile(x - 1, y + 1));
        if (board.FindTile(x - 1, y) != null) tiles.Add(board.FindTile(x - 1, y));
        if (board.FindTile(x - 1, y - 1) != null) tiles.Add(board.FindTile(x - 1, y - 1));
        return tiles;
    }
    public Tile(Tile cloneTile)
    {
        x = cloneTile.x;
        y = cloneTile.y;
    }
    public Tile(int x, int y, TileObject tileObject)
    {
        this.x = x;
        this.y = y;
        this.tileObject = tileObject;
    }
}
