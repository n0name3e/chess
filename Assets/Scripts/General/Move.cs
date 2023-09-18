public class Move
{
    public Tile movedTile;
    private Tile startingTile;

    public Piece movedPiece;
    public Piece capturedPiece;

    private Board board;
    private System.Collections.Generic.List<Piece> piecesList;


    public void MakeMove(Tile tileToMove, Board board)
    {
        if (movedPiece.currentTile == null) return; // has been beaten
        if (tileToMove.CurrentPiece != null)
        {
            piecesList = new System.Collections.Generic.List<Piece>(board.pieces);
            capturedPiece = tileToMove.CurrentPiece;
        }
        movedPiece.MovePiece(tileToMove.x, tileToMove.y, board);
        movedTile = tileToMove;
        this.board = board;
    }
    public void UndoMove()
    {
        if (movedPiece.currentTile == null) return;
        if (startingTile.CurrentPiece != null)
        {
            UnityEngine.MonoBehaviour.print(startingTile.CurrentPiece == null);
        }
        movedPiece.MovePiece(startingTile.x, startingTile.y, board);
        if (capturedPiece != null)
        {
            board.pieces = piecesList;
            capturedPiece.currentTile = movedTile;
            capturedPiece.currentTile.CurrentPiece = capturedPiece;
        }
    }
    public Move(Piece movedPiece)
    {        
        this.movedPiece = movedPiece;
        startingTile = movedPiece.currentTile;
    }
}
