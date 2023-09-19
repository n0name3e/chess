using System.Collections.Generic;
using UnityEngine;

public class Abilities : MonoBehaviour
{
    private static Abilities instance;

    public static Abilities Instance { get => instance; set => instance = value; }
    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        //StartCoroutine(a());
        if (GameManager.instance.showAbilities)
        {
            a(BoardCreator.mainBoard);
        }
    }
    public static void JAA()
    {
        /*List<Ability> abilities = new List<Ability>
        {
            new Ability("weaker") { OnUntargetedAbilityChoose = UseWeaker },
            new Ability("fireball") { OnTargetedAbilityChoose = UseFireball, OnTileChoose = CastFireball },
            new Ability("teleport") { OnTargetedAbilityChoose = UseTeleport, OnTileChoose = CastTeleport }

        };*/
    }
    public void a(Board board) //System.Collections.IEnumerator a()
    {
        //yield return new WaitForEndOfFrame();

        foreach (Piece p in board.pieces)
        {
            p.abilities.Add(new Ability("weaker", p) { OnUntargetedAbilityChoose = UseWeaker });
            p.abilities.Add(new Ability("fireball", p) { OnTargetedAbilityChoose = UseFireball, OnTileChoose = CastFireball });
            if (p.pieceType != PieceType.Pawn)
            {
                p.abilities.Add(new Ability("teleport", p) { OnTargetedAbilityChoose = UseTeleport, OnTileChoose = CastTeleport, OnBotChoose = UseBotTeleport });
            }
            p.abilities.Add(new Ability("doubleStep", p) { OnUntargetedAbilityChoose = UseDoubleStep });
            p.abilities.Add(new Ability("hex", p) { OnStart = InitHex, OnUntargetedAbilityChoose = UseHex });
            p.abilities.Add(new Ability("kamikadze", p) { OnStart = InitKamikadze });
            p.abilities.Add(new Ability("shadowStep", p) { OnTargetedAbilityChoose = UseShadowStep, OnTileChoose = CastShadowStep });
            p.abilities.Add(new Ability("mindControl", p) { OnTargetedAbilityChoose = UseMindControl, OnTileChoose = CastMindControl });
            p.abilities.Add(new Ability("punch", p) { OnUntargetedAbilityChoose = UsePunch });
        }
    }
    bool UseWeaker(Piece piece) // static will be removed!
    {
        List<Tile> tiles = piece.GetPossibleMoves(false, false);
        bool wasCasted = false;
        foreach (Tile t in tiles)
        {
            if (t.CurrentPiece != null && t.CurrentPiece.color != piece.color)
            {
                wasCasted = true;
                void Add(Piece p)
                {
                    p.health *= 1f - AbilityManager.Instance.FindAbility("weaker").GetProperty("healthDecreasing");
                    p.maxHealth *= 1f - AbilityManager.Instance.FindAbility("weaker").GetProperty("healthDecreasing");
                }
                void Remove(Piece p)
                {
                    p.maxHealth /= 1f - AbilityManager.Instance.FindAbility("weaker").GetProperty("healthDecreasing");
                    p.health /= 1f - AbilityManager.Instance.FindAbility("weaker").GetProperty("healthDecreasing");
                }
                Buff weak = new Buff("weakerBuff", (int)AbilityManager.Instance.FindAbility("weaker").GetProperty("duration"))
                {
                    OnAddBuff = Add,
                    OnRemoveBuff = Remove
                };
                BuffManager.Instance.AddBuff(t.CurrentPiece, weak);
                /*t.CurrentPiece.health *= (1f - AbilityManager.Instance.FindAbility("weaker").GetProperty("healthDecreasing"));  //(1f - (float)AbilityManager.Instance.FindAbility("weaker"));
                t.CurrentPiece.maxHealth *= (1f - AbilityManager.Instance.FindAbility("weaker").GetProperty("healthDecreasing")); //(1f - (float)AbilityManager.Instance.FindAbility("weaker").properties);
                */
            }
        }
        return wasCasted;
    }
    List<Tile> UseFireball(Piece piece)
    {
        List<Tile> possibleMoves = piece.GetPossibleMoves();
        List<Tile> moves = new List<Tile>();
        foreach (Tile t in possibleMoves)
        {
            if (t.CurrentPiece != null && t.CurrentPiece.color != piece.color) moves.Add(t);
        }
        return moves;
    }
    void CastFireball(Tile tile, Piece piece)
    {
        tile.CurrentPiece.DealDamage(AbilityManager.Instance.FindAbility("fireball").GetProperty("damage"));
    }
    List<Tile> UseTeleport(Piece piece)
    {
        if (TileSelector.Instance.doubleMovePiece != null) return new List<Tile>(0);
        List<Tile> moves = new List<Tile>();
        foreach (Tile t in piece.board.tiles)
        {
            if (t.CurrentPiece != null && t.CurrentPiece.color == piece.color && t.CurrentPiece.pieceType == PieceType.Pawn) moves.Add(t);
        }
        return moves;
    }
    public (float, Tile) UseBotTeleport(Board board, Ability ability)
    {
        float bestEval = int.MaxValue;
        float eval = 0.0f;
        Tile bestTile = null;

        Piece piece = ability.owner;
        List<Tile> moves = ability.OnTargetedAbilityChoose?.Invoke(piece);
        foreach (Tile tile in moves)
        {
            Board cloneBoard = new Board();
            cloneBoard.CopyPieces(board.pieces);
            Tile t = cloneBoard.FindTile(tile.x, tile.y);
            Piece p = cloneBoard.FindTile(piece.x, piece.y).CurrentPiece;
            Ability cloneAbility = cloneBoard.FindTile(piece.x, piece.y)?.CurrentPiece.FindAbility(ability.name);
            if (cloneAbility == null) continue;
            cloneAbility.OnTileChoose?.Invoke(tile, p);

            eval = ChessAI.Instance.Minimax(cloneBoard, 1, int.MinValue, int.MaxValue, false);
            if (eval < bestEval)
            {
                bestEval = eval;
                bestTile = tile;
            }
        }
        return (bestEval, bestTile);
        /*foreach (Piece piece in new List<Piece>(board.pieces))
        {
            if (piece.color == Colors.White) continue;
            Ability ability = piece.FindAbility("teleport");

            if (ability == null || !ability.canBeCasted()) continue;

            if (ability.OnTargetedAbilityChoose != null)
            {
                moves = ability.OnTargetedAbilityChoose.Invoke(piece);
                if (ability.OnTileChoose == null) continue;
                foreach (Tile t in moves)
                {
                    Board cloneBoard = new Board();
                    cloneBoard.CopyPieces(board.pieces);
                    Tile tile = cloneBoard.FindTile(t.x, t.y);
                    Piece p = cloneBoard.FindTile(piece.x, piece.y).CurrentPiece;

                    Ability cloneAbility = cloneBoard.FindTile(piece.x, piece.y)?.CurrentPiece.FindAbility(ability.name);
                    if (cloneAbility == null) continue;
                    cloneAbility.OnTileChoose?.Invoke(tile, p);
                    manaCostEval = ability.manaCost * 0.005f;

                    bestEval = Mathf.Min(ChessAI.Instance.Minimax(cloneBoard, 1, int.MinValue, int.MaxValue, false) - 1, bestEval);
                    bestAbility = ability;
                    bestTile = tile;
                }
            }

        }
        if (bestTile != null)
        {
            bestTile = board.FindTile(bestTile.x, bestTile.y);
        }*/
        //return (bestEval, bestAbility, bestTile);
    }

    void CastTeleport(Tile tile, Piece castingPiece)
    {
        castingPiece.MovePiece(tile.x, tile.y, castingPiece.board);
    }
    bool UseDoubleStep(Piece piece)
    {
        if (TileSelector.Instance.doubleMovePiece != null) return false;
        TileSelector.Instance.doubleMovePiece = piece;
        Buff buff = new Buff("doubleMove", 1);
        buff.isUnlimited = true;
        buff.AddCharge(2);
        BuffManager.Instance.AddBuff(piece, buff);
        return true;
    }
    void InitHex(Piece piece)
    {
        void Check(Piece p, Colors color)
        {
            Buff buff = new Buff("checkHex", 3);
            BuffManager.Instance.AddBuff(p, buff);
        }
        GameManager.instance.Delegates.OnCheck += Check;
    }
    bool UseHex(Piece piece)
    {
        Piece originalPiece = null;
        Piece hexedPiece = null;
        bool hasBeenCasted = false;
        void UnTransform(Piece pi)
        {
            hexedPiece.currentTile.ChangeCurrentPiece(originalPiece);
            originalPiece.board.pieces.Add(originalPiece);
            if (originalPiece.pieceObject != null)
            {
                originalPiece.pieceObject.transform.position = new Vector3(originalPiece.x, 0.02f, originalPiece.y);
            }
        }
        Buff hex = new Buff("hex", 2)
        {
            //OnAddBuff = Transform,
            OnRemoveBuff = UnTransform
        };
        foreach (Piece p in new List<Piece>(BoardCreator.mainBoard.pieces))
        {
            foreach (Buff buff in new List<Buff>(p.buffs))
            {
                if (BuffManager.Instance.FindBuff(p, "checkHex") != null)
                {
                    hasBeenCasted = true;
                    Tile tile = p.currentTile;
                    originalPiece = tile.CurrentPiece;
                    originalPiece.currentTile.CurrentPiece = null;
                    originalPiece.currentTile = null;
                    originalPiece.board.pieces.Remove(originalPiece);
                    if (originalPiece.pieceObject != null)
                    {
                        originalPiece.pieceObject.gameObject.transform.position = new Vector3(102, 129, 921);
                        tile.SpawnPiece(Resources.Load<GameObject>("PawnBlack"));
                    }
                    BuffManager.Instance.AddBuff(tile.CurrentPiece, hex);
                    hexedPiece = tile.CurrentPiece;
                }
            }
        }
        return hasBeenCasted;
    }
    void InitKamikadze(Piece piece)
    {
        void Kamikadze(Piece capturer, Piece captured)
        {
            if (piece.x == capturer.x && piece.y == capturer.y)
            {
                Board board = BoardCreator.mainBoard;
                foreach (Piece p in new List<Piece>(BoardCreator.mainBoard.pieces))
                {
                    AbilityData ability = AbilityManager.Instance.FindAbility("kamikadze");
                    if (p.color == capturer.color || p.pieceType == PieceType.King) continue;
                    p.DealDamage(ability.GetProperty("basicDamage") + capturer.cost * ability.GetProperty("damagePerCost"));
                }
            }
        }
        GameManager.instance.Delegates.OnCapture += Kamikadze;

    }
    List<Tile> UseShadowStep(Piece piece)
    {
        List<Tile> tiles = new List<Tile>();
        foreach (Piece p in piece.board.pieces)
        {
            if (p.color == piece.color) continue;
            Tile tile = piece.board.FindTile(p.x, p.y + 1);
            if (tile == null || tile.CurrentPiece != null) continue;
            foreach (Tile t in p.currentTile.GetNeighbourTiles())
            {
                if (t.CurrentPiece != null && t.CurrentPiece.color != piece.color) goto A;
            }
            if (tile != null) tiles.Add(tile);
            A: continue;
        }
        return tiles;
    }
    void CastShadowStep(Tile tile, Piece piece)
    {
        tile.ChangeCurrentPiece(piece);
    }
    List<Tile> UseMindControl(Piece piece)
    {
        List<Tile> tiles = new List<Tile>();
        foreach (Piece p in piece.board.pieces)
        {
            if (p.color == piece.color || p.pieceType == PieceType.King) continue;
            tiles.Add(p.currentTile);
        }
        return tiles;
    }
    void CastMindControl(Tile tile, Piece piece)
    {
        Piece originalPiece = tile.CurrentPiece;
        if (originalPiece.pieceObject == null) return;
        Piece transformedPiece = null;
        originalPiece.currentTile.CurrentPiece = null;
        originalPiece.currentTile = null;
        originalPiece.board.pieces.Remove(originalPiece);
        if (originalPiece.pieceObject != null)
        {
            originalPiece.pieceObject.gameObject.transform.position = new Vector3(102, 129, 921);
            tile.SpawnPiece(Resources.Load<GameObject>("Queen"));
        }
        transformedPiece = tile.CurrentPiece;
        void UnTransform(Piece p)
        {
            transformedPiece.currentTile.ChangeCurrentPiece(originalPiece);
            originalPiece.board.pieces.Add(originalPiece);
            if (originalPiece.pieceObject != null)
            {
                originalPiece.pieceObject.transform.position = new Vector3(originalPiece.x, 0.02f, originalPiece.y);
            }
        }
        void FireRocket(Tile t, Piece p)
        {
            if (t.CurrentPiece != null)
            {
                t.CurrentPiece.DealDamage(AbilityManager.Instance.FindAbility("mindControl").GetProperty("rocketDamage"));
                p.shouldMove = false;
            }
        }
        Buff buff = new Buff("transform", 3) { OnRemoveBuff = UnTransform };
        if (transformedPiece != null)
        {
            BuffManager.Instance.AddBuff(transformedPiece, buff);
            transformedPiece.OnMove = FireRocket;
        }
    }
    bool UsePunch(Piece piece)
    {
        Tile pieceTile = piece.board.FindTile(piece.x, piece.y + 1);
        Piece pieceToPunch = pieceTile.CurrentPiece;
        Tile tileToPunch = piece.board.FindTile(piece.x, piece.y + 2);
        if (pieceTile != null && pieceToPunch != null && pieceToPunch.color != piece.color)
        {
            if (tileToPunch != null && tileToPunch.CurrentPiece == null)
            {
                pieceToPunch.MovePiece(tileToPunch.x, tileToPunch.y);
                pieceToPunch.DealDamage(AbilityManager.Instance.FindAbility("punch").GetProperty("damage"));
            }
            else if (tileToPunch != null && tileToPunch.CurrentPiece != null)
            {
                pieceToPunch.DealDamage(AbilityManager.Instance.FindAbility("punch").GetProperty("damage") + AbilityManager.Instance.FindAbility("punch").GetProperty("additionalDamage"));
            }
        }
        else
        {
            return false;
        }

        return true;
    }
}
