using System.Collections.Generic;
using UnityEngine;

public class ChessAI: MonoBehaviour
{
    private static ChessAI instance;

    [SerializeField] private UnityEngine.UI.Text textEval;
    [SerializeField] private bool useAbility = true;

    public int MaxDepth = 2;
    private int movesMade = 0;
    private int branchesRemoved = 0;
    private Dictionary<PieceType, int> piecesMoved = new Dictionary<PieceType, int>();
    public static ChessAI Instance { get => instance; private set => instance = value; }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        piecesMoved.Add(PieceType.Pawn, 0);
        piecesMoved.Add(PieceType.Knight, 0);
        piecesMoved.Add(PieceType.Bishop, 0);
        piecesMoved.Add(PieceType.Rook, 0);
        piecesMoved.Add(PieceType.Queen, 0);
        piecesMoved.Add(PieceType.King, 0);

    }
    public void MakeMove()
    {
        System.DateTime startingTime = System.DateTime.Now;
        Tile bestMove = null;
        Piece bestPiece = null;
        float bestEvalution = float.MaxValue;
        float alpha = float.MinValue;
        float beta = float.MaxValue;
        float startingEvalution;
        Dictionary<Piece, Piece> pieces = new Dictionary<Piece, Piece>();

        Board cloneBoard = new Board();
        cloneBoard.CopyPieces(BoardCreator.mainBoard.pieces);

        if (useAbility)
        {
            (float bestE, Ability bestAbility, Tile bestT) = CheckAbilities(cloneBoard);
            if (bestAbility != null)
            {
                Piece bestAbilityPiece = BoardCreator.mainBoard.FindTile(bestAbility.owner.x, bestAbility.owner.y).CurrentPiece;
                Tile bestAbilityPieceTile = bestAbilityPiece.currentTile;
                AbilityManager.Instance.CastAbility(bestAbility, bestT, bestAbility.owner);
                Tile t = BoardCreator.mainBoard.FindTile(bestT.x, bestT.y);
                print(bestAbilityPiece.pieceType);
                AbilityManager.Instance.CastAbility(bestAbilityPiece.FindAbility(bestAbility.name), t, bestAbilityPiece);
                TileSelector.Instance.SetMovedTiles((bestAbilityPieceTile, BoardCreator.mainBoard.FindTile(bestT.x, bestT.y)));
            }
        }

        startingEvalution = Evaluate(cloneBoard);
        List<Piece> p = BoardCreator.mainBoard.pieces;
        for (int i = 0; i < p.Count; i++)
        {
            if (BoardCreator.mainBoard.FindTile(p[i].x, p[i].y).CurrentPiece == null || cloneBoard.FindTile(p[i].x, p[i].y).CurrentPiece == null) continue;
            pieces.Add(cloneBoard.FindTile(p[i].x, p[i].y).CurrentPiece, BoardCreator.mainBoard.FindTile(p[i].x, p[i].y).CurrentPiece);   
        }
        List<Piece> clonedPieces = new List<Piece>(cloneBoard.pieces);
        foreach (Piece piece in clonedPieces)
        {
            startingEvalution = Evaluate(cloneBoard) - (piece.GetSquareTable(piece.x, piece.y) / 100);
            if (piece.color == Colors.White) continue;
            List<Tile> possibleMoves = piece.GetPossibleMoves(false); // todo? checking check
            foreach (Tile tilee in possibleMoves)
            {
                movesMade++;
                int x = piece.x;
                int y = piece.y;
                Tile startingTile = piece.currentTile;
                Tile tile = cloneBoard.FindTile(tilee.x, tilee.y);
                //if (tile.CurrentPiece != null && piece.color == tile.CurrentPiece.color) continue;    
                if (tile.CurrentPiece != null && tile.CurrentPiece.isProtecredByPawn()
                    && piece.pieceType != PieceType.Pawn && bestMove == null)
                {
                    branchesRemoved++;
                    continue;
                }
                piecesMoved[piece.pieceType] += 1;
                Move move = new Move(piece);//(cloneBoard.FindTile(piece.x, piece.y).CurrentPiece);
                move.MakeMove(tile, cloneBoard);
                float e = Evaluate(cloneBoard) - (piece.GetSquareTable(piece.x, piece.y) / 100);
                if (e >= startingEvalution &&
                    bestEvalution < e)
                {
                    //print(piece.pieceType + " (" + tile.x + "; " + tile.y + ") " + startingEvalution + " -> " + e + " : Black");
                    //print("remove (black)");
                    branchesRemoved++;
                    move.UndoMove();
                    continue;
                }
                float eval = Minimax(cloneBoard, MaxDepth, alpha, beta, true)
                    - piece.GetSquareTable(tile.x, tile.y) / 100
                    + piece.GetSquareTable(x, y) / 100;
                float bonusEval = EvaluatePiece(cloneBoard, piece, startingTile, tilee);
                eval -= bonusEval;
                //print($"{piece} {x}; {y} -> {tile.x}; {tile.y}. -{piece.GetSquareTable(tile.x, tile.y) / 100} + {piece.GetSquareTable(x, y) / 100}");
                //eval *= -1;
                if (eval < bestEvalution || bestPiece == null)
                {
                    if (!GameManager.instance.KingInCheck(cloneBoard, Colors.Black).check)
                    {
                        bestMove = tile;
                        bestPiece = piece;
                        bestEvalution = eval;
                    }
                }
                move.UndoMove();

                //alpha = Mathf.Max(alpha, bestEvalution);
                //if (beta < alpha) break;
            }
        }

        cloneBoard.DestroyBoard();
        if (bestPiece == null)
        {
            print("checkmate or stalemate <3 ^-^");
            GameManager.instance.GameOver();
            return;
        }
        Piece piece1 = pieces[bestPiece]; //BoardCreator.mainBoard.FindTile(bestPiece.x, bestPiece.y).CurrentPiece;
        Move moving = new Move(piece1);
        Tile tileToMove = BoardCreator.mainBoard.FindTile(bestMove.x, bestMove.y);
        moving.MakeMove(tileToMove, BoardCreator.mainBoard);
        System.DateTime endingTime = System.DateTime.Now;
        System.TimeSpan thinkingTime = endingTime - startingTime;
        textEval.text = "Eval: " + bestEvalution.ToString()
            + "\n Moves Made:" + movesMade
            + "\n Branches Removed:" + branchesRemoved
            + "\n Pawn:" + piecesMoved[PieceType.Pawn]
            + "\n Knight:" + piecesMoved[PieceType.Knight]
            + "\n Bishop:" + piecesMoved[PieceType.Bishop]
            + "\n Rook:" + piecesMoved[PieceType.Rook]
            + "\n Queen:" + piecesMoved[PieceType.Queen]
            + "\n King:" + piecesMoved[PieceType.King]
            + "\n Time elapsed:" + thinkingTime.TotalMilliseconds + " ms";
        movesMade = 0;
        branchesRemoved = 0;
        piecesMoved[PieceType.Pawn] = 0;
        piecesMoved[PieceType.Knight] = 0;
        piecesMoved[PieceType.Bishop] = 0;
        piecesMoved[PieceType.Rook] = 0;
        piecesMoved[PieceType.Queen] = 0;
        piecesMoved[PieceType.King] = 0;
        GameManager.instance.EndTurn();
    }
    
    public float Minimax(Board board, int depth, float alpha, float beta, bool maximizingPlayer)
    {
        if (depth <= 0)
            return Evaluate(board);

        List<Piece> pieces = new List<Piece>(board.pieces);
        float startingEvalution;
        if (maximizingPlayer)  // white
        {
            float bestEval = float.MinValue;
            foreach (Piece piece in pieces)
            {
                startingEvalution = Evaluate(board) + (piece.GetSquareTable(piece.x, piece.y) / 100);
                if (piece.color == Colors.Black) continue;
                List<Tile> poss = piece.GetPossibleMoves(true);
                foreach (Tile tile in poss)
                {
                    movesMade++;
                    int x = piece.x;
                    int y = piece.y;
                    Tile move = board.FindTile(tile.x, tile.y);
                    Tile startingTile = piece.currentTile;

                    if (tile.CurrentPiece != null && tile.CurrentPiece.isProtecredByPawn()
                        && piece.pieceType != PieceType.Pawn && bestEval == float.MinValue)
                    {
                        branchesRemoved++;
                        continue;
                    }
                    Move moveObj = new Move(piece);
                    moveObj.MakeMove(move, board);
                    float e = Evaluate(board) + (piece.GetSquareTable(piece.x, piece.y) / 100);
                    if (e <= startingEvalution &&
                        bestEval >= e)
                    {
                        moveObj.UndoMove();
                        branchesRemoved++;
                        continue;
                    }
                    piecesMoved[piece.pieceType] += 1;

                    float eval = Minimax(board, depth - 1, alpha, beta, false)
                        + piece.GetSquareTable(tile.x, tile.y) / 100
                        - piece.GetSquareTable(x, y) / 100;
                    float bonusEval = EvaluatePiece(board, piece, startingTile, tile);
                    eval += bonusEval;
                    if (eval > bestEval)
                    {
                        if (!GameManager.instance.KingInCheck(board, Colors.White).check)
                        {
                            bestEval = eval;
                        }
                    }
                    moveObj.UndoMove();
                }
            }
            return bestEval;
        }
        else  // black
        {
            float bestEval = float.MaxValue;
            foreach (Piece piece in pieces)
            {
                if (piece.color == Colors.White) continue;
                foreach (Tile tile in piece.GetPossibleMoves(false))
                {
                    movesMade++;
                    int x = piece.x;
                    int y = piece.y;
                    Tile move = board.FindTile(tile.x, tile.y);
                    Tile startingTile = piece.currentTile;
                    if (tile.CurrentPiece != null && tile.CurrentPiece.isProtecredByPawn()
                        && piece.pieceType != PieceType.Pawn && bestEval == float.MaxValue)
                    {
                        branchesRemoved++;
                        continue;
                    }
                    piecesMoved[piece.pieceType] += 1;

                    Move moveObj = new Move(piece);
                    moveObj.MakeMove(move, board);
                    float eval = Minimax(board, depth - 1, alpha, beta, true)
                        - piece.GetSquareTable(tile.x, tile.y) / 100 
                        + piece.GetSquareTable(x, y) / 100;
                    float bonusEval = EvaluatePiece(board, piece, startingTile, tile);
                    eval -= bonusEval;
                    if (eval < bestEval)
                    {
                        if (!GameManager.instance.KingInCheck(board, Colors.Black).check)
                        {
                            bestEval = eval;
                        }
                    }
                    moveObj.UndoMove();
                }
            }
            return bestEval;
        }
    }

    public (float, Ability, Tile) CheckAbilities(Board board)
    {
        float initialEval = Evaluate(board);
        float eval = 0.0f;

        float bestEval = int.MaxValue;
        Ability bestAbility = null;
        Tile tile = null;
        Tile bestTile = null;
        foreach (Piece piece in board.pieces)
        {
            if (piece.color == Colors.White) continue;
            foreach (Ability ability in piece.abilities)
            {
                if (!BoardCreator.mainBoard.FindTile(piece.x, piece.y).CurrentPiece.FindAbility(ability.name).canBeCasted()) continue;
                if (ability.OnBotChoose != null)
                {
                    (eval, tile) = ability.OnBotChoose.Invoke(board, ability);
                    if (eval < bestEval && (eval + 0.5f) < initialEval)
                    {
                        bestEval = eval;
                        bestAbility = ability;
                        bestTile = board.FindTile(tile.x, tile.y);
                    }
                }
            }
        }      
        return (bestEval, bestAbility, bestTile);
    }

    /// <summary>
    /// returns additional eval based on position of piece
    /// </summary>
    /// <returns></returns>
    public float EvaluatePiece(Board board, Piece evaluatedPiece, Tile startingTile, Tile movedTile)
    {
        float eval = 0.0f;

        // doublePawns and opposition
        if (evaluatedPiece.pieceType == PieceType.Pawn)
        {
            int forwardDirection = (evaluatedPiece.color == Colors.White) ? 1 : -1;
            Tile tile = board.FindTile(movedTile.x, movedTile.y + forwardDirection);
            if (tile?.CurrentPiece != null)
            {
                if (tile.CurrentPiece.color != evaluatedPiece.color)
                {
                    eval += 0.35f;
                }
            }
            tile = board.FindTile(movedTile.x, movedTile.y - forwardDirection);
            if (tile?.CurrentPiece != null && tile.CurrentPiece.color == evaluatedPiece.color)
            {
                eval -= 0.25f;
            }
        }

        // bonus points for attacking piece (dont work for queen to not evaluate too much)
        if (evaluatedPiece.pieceType != PieceType.Queen && board.pieces.Count > 12) return eval;
        List<Tile> possibleMoves = evaluatedPiece.GetPossibleMoves();
        foreach (Tile move in possibleMoves)
        {
            if (move.CurrentPiece != null && move.CurrentPiece.pieceType != PieceType.Pawn && move.CurrentPiece.color != evaluatedPiece.color)
            {
                eval += 0.15f;
            }
        }

        // - points for moving at square controlled by pawn
        int pawnForwardDirection = (evaluatedPiece.color == Colors.White) ? 1 : -1;
        Tile checkingTile = board.FindTile(evaluatedPiece.x + 1, evaluatedPiece.y + pawnForwardDirection);
        Tile checkingTile1 = board.FindTile(evaluatedPiece.x - 1, evaluatedPiece.y + pawnForwardDirection);
        if (checkingTile?.CurrentPiece?.color != evaluatedPiece.color || checkingTile1?.CurrentPiece?.color != evaluatedPiece.color)
        {
            if (evaluatedPiece.pieceType == PieceType.Pawn)
            {
                bool protectingPawn0 = board.FindTile(evaluatedPiece.x + 1, evaluatedPiece.y - pawnForwardDirection).CurrentPiece?.color == evaluatedPiece.color;
                bool protectingPawn1 = board.FindTile(evaluatedPiece.x - 1, evaluatedPiece.y - pawnForwardDirection).CurrentPiece?.color == evaluatedPiece.color;
                if (protectingPawn0 || protectingPawn1)
                {
                    print("2991");
                    eval += 1f;
                }         
            }
            else
            {
                eval -= 0.3f;
            }
        }
        return eval;
    }
    public int Evaluate(Board board)
    {
        List<Piece> pieces = board.pieces;
        int whitePieceCost = 0;
        int blackPieceCost = 0;
        foreach (Piece piece in pieces)
        {
            if (piece.color == Colors.White)
            {
                whitePieceCost += piece.cost;
            }
            else
            {
                blackPieceCost += piece.cost;
            }
        }
        return whitePieceCost - blackPieceCost;
    }
}
