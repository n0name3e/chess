using System.Collections;
using UnityEngine;

public class PieceObject : MonoBehaviour
{
    public Piece piece;
    public Colors color;
    public PieceType pieceType;


    private void OnEnable()
    { 
        piece = new Piece(this);
        if (pieceType == PieceType.Pawn) new Pawn().Enable(piece);
        if (pieceType == PieceType.Knight) new Knight().Enable(piece);
        if (pieceType == PieceType.Bishop) new Bishop().Enable(piece);
        if (pieceType == PieceType.Rook) new Rook().Enable(piece);
        if (pieceType == PieceType.Queen) new Queen().Enable(piece);
        if (pieceType == PieceType.King) new King().Enable(piece);

        piece.color = color;
    }
}
