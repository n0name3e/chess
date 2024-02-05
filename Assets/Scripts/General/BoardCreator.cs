using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

public class BoardCreator : MonoBehaviour
{
    private static BoardCreator instance;

    [SerializeField] private int boardSize = 8;
    [SerializeField] private Transform cont;
    public static Board mainBoard;
    private Transform container;
    public static Colors playerColor = Colors.White;

    #region Pieces
    private GameObject pawn;
    private GameObject knight;
    private GameObject bishop;
    private GameObject rook;
    [HideInInspector] public static GameObject queen;
    private GameObject king;

    private GameObject blackPawn;
    private GameObject blackKnight;
    private GameObject blackBishop;
    private GameObject blackRook;
    [HideInInspector] public static GameObject blackQueen;
    private GameObject blackKing;
    #endregion

    public Color whiteColor = Color.white;
    public Color blackColor = Color.black;
    public Color selectedColor = Color.yellow;
    public Color highlightedColor = Color.green;
    public Color abilityHighlighedColorEnemy = Color.red;
    public Color abilityHighlighedColorFriend = Color.yellow;

    public static BoardCreator Instance { get => instance; private set => instance = value; }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }


        king = Resources.Load<GameObject>("King");
        queen = Resources.Load<GameObject>("Queen");
        rook = Resources.Load<GameObject>("Rook");
        bishop = Resources.Load<GameObject>("Bishop");
        knight = Resources.Load<GameObject>("Knight");
        pawn = Resources.Load<GameObject>("Pawn");

        blackPawn = Resources.Load<GameObject>("PawnBlack");
        blackKnight = Resources.Load<GameObject>("KnightBlack");
        blackBishop = Resources.Load<GameObject>("BishopBlack");
        blackRook = Resources.Load<GameObject>("RookBlack");
        blackQueen = Resources.Load<GameObject>("QueenBlack");
        blackKing = Resources.Load<GameObject>("KingBlack");
    }
    void Start()
    {
        container = Instantiate(new GameObject("Tiles")).transform;
        GameManager.Instance.StartGame();
    }

    public static Tile FindTile(int x, int y)
    {
        return mainBoard.tiles[(x-1) * 8 + (y - 1)];
    }
    public void DestroyMainBoard()
    {
        foreach (Tile t in mainBoard.tiles)
        {
            Destroy(t.tileObject.gameObject);
        }
        foreach (Piece p in mainBoard.pieces)
        {
            Destroy(p.pieceObject.gameObject);
        }
        mainBoard.DestroyBoard();
    }
    public void SpawnMainBoard()
    {
        List<Tile> tiles = new List<Tile>(64);
        for (int x = 1; x <= boardSize; x++)
        {
            for (int y = 1; y <= boardSize; y++)
            {
                GameObject tile = Instantiate(Resources.Load<GameObject>("Tile"), new Vector3(x, 0, y), Quaternion.identity);
                tile.transform.SetParent(container);

                TileObject tileObject = tile.GetComponent<TileObject>();
                tileObject.x = x;
                tileObject.y = y;
                tileObject.SetDefaultColor();

                Tile t = tileObject.tile;
                tile.transform.name = $"{x}; {y}";
                t.x = x;
                t.y = y;
                tiles.Add(t);
            }
        }
        mainBoard = new Board();
        mainBoard.tiles = tiles;
        foreach (Tile tile in mainBoard.tiles)
        {
            tile.board = mainBoard;
        }
        mainBoard.name = "very main board"; // for debug with wrong board evaluating
        SpawnAllPieces();
    }
    
    private void SpawnAllPieces()
    {
        // debug position

        /* FindTile(5, 1).SpawnPiece(king);
        FindTile(5, 8).SpawnPiece(blackKing);
        FindTile(2, 6).SpawnPiece(blackKnight);
        FindTile(1, 5).SpawnPiece(bishop);
        FindTile(6, 6).SpawnPiece(blackQueen);
        FindTile(1, 7).SpawnPiece(blackPawn);
        FindTile(2, 7).SpawnPiece(blackPawn);*/

        /*FindTile(4, 8).SpawnPiece(blackKing); // ..bar haha */

        FindTile(1, 3).SpawnPiece(blackPawn);
        FindTile(1, 1).SpawnPiece(rook);
        FindTile(2, 1).SpawnPiece(knight);
        FindTile(3, 1).SpawnPiece(bishop);
        FindTile(4, 1).SpawnPiece(queen);
        FindTile(5, 1).SpawnPiece(king);
        FindTile(6, 1).SpawnPiece(bishop);
        FindTile(7, 1).SpawnPiece(knight);
        FindTile(8, 1).SpawnPiece(rook);
        for (int x = 1; x <= 8; x++) FindTile(x, 2).SpawnPiece(pawn);

        FindTile(1, 8).SpawnPiece(blackRook);
        FindTile(2, 8).SpawnPiece(blackKnight);
        FindTile(3, 8).SpawnPiece(blackBishop);
        FindTile(4, 8).SpawnPiece(blackQueen);
        FindTile(5, 8).SpawnPiece(blackKing); // ..bar haha
        FindTile(6, 8).SpawnPiece(blackBishop);
        FindTile(7, 8).SpawnPiece(blackKnight);
        FindTile(8, 8).SpawnPiece(blackRook);

        for (int x = 1; x <= 8; x++) FindTile(x, 7).SpawnPiece(blackPawn);
    }
}
