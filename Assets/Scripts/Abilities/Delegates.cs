public class Delegates
{
    public delegate void Check(Piece piece, Colors color);
    public Check OnCheck;

    public delegate void Capture(Piece capturerPiece, Piece capturedPiece);
    public Capture OnCapture;

    public delegate void EndTurn(Colors currentTurn);
    public EndTurn OnTurnEnd;

    public void Clear()
    {
        OnCheck = null;
        OnCapture = null;
    }
}
