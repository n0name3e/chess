using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

public class TileSelector : MonoBehaviour
{
    private static TileSelector instance;

    private Camera cam;
    private TileObject selectedTileObject;

    public Piece selectedPiece;
    private Ability selectedAbility;

    private Tile selectedTile;
    List<Tile> possibleTileMoves = new List<Tile>();

    public Piece doubleMovePiece;
    [SerializeField] private bool isAndroid = false;
    [SerializeField] private UnityEngine.UI.Text text;
    [SerializeField] private UnityEngine.UI.Text tooltip;

    private List<TileObject> highlightedTiles = new List<TileObject>();
    private List<Tile> abilityTiles = new List<Tile>();
    int a;

    public static TileSelector Instance { get => instance; set => instance = value; }

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }
    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.GameEnded) return;
        if (isAndroid && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            CheckTile();
            ShowStats();
            return;
        }
        if (Input.GetMouseButtonDown(0))
        {
            CheckTile();
        }
        if (Input.GetMouseButtonDown(1))
        {
            ShowStats();
        }
    }
    private void ShowStats()
    {
        selectedAbility = null;
        //UnhighlightTiles();
        Ray ray;
        if (isAndroid)
        {
            ray = cam.ScreenPointToRay(Input.GetTouch(0).position);
        }
        else
        {
            ray = cam.ScreenPointToRay(Input.mousePosition);
        }
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            TileObject tileObject = hit.transform.GetComponent<TileObject>();
            if (tileObject != null)
            {
                Tile tile = tileObject.tile;
                
                if (tile.CurrentPiece != null)
                {
                    SetSelectedPiece(tile.CurrentPiece);                    
                }
            }
        }
    }
    private void CheckTile()
    {
        if (doubleMovePiece != null && BuffManager.Instance.FindBuff(doubleMovePiece, "doubleMove") == null) doubleMovePiece = null;
        Ray ray;
        if (isAndroid)
        {
            if (Input.touchCount <= 0)
            {
                return;
            }
            ray = cam.ScreenPointToRay(Input.GetTouch(0).position);
        }
        else
        {
            ray = cam.ScreenPointToRay(Input.mousePosition);
        }
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            TileObject tileObject = hit.transform.GetComponent<TileObject>();
            if (tileObject != null)
            {
                Tile tile = tileObject.tile;
                if (abilityTiles.Contains(tile))
                {
                    AbilityManager.Instance.CastAbility(selectedAbility, tile, selectedPiece);
                    SetSelectedAbility(null);
                    UnhighlightTiles();
                    return;
                }
                if (selectedAbility != null)
                {
                    SetSelectedAbility(null);
                    selectedPiece = null;
                    UnhighlightTiles();
                }
                if (doubleMovePiece != null && tileObject.tile.CurrentPiece != null
                    && tileObject.tile.CurrentPiece != doubleMovePiece)
                    
                    //&& tileObject.tile.CurrentPiece != null && tileObject.tile.CurrentPiece.color == selectedTileObject.tile.CurrentPiece.color) 
                {
                    return;
                }
                if (selectedTileObject == null && tile.CurrentPiece != null && tile.CurrentPiece.color != BoardCreator.playerColor)
                    return;
                if (tile.CurrentPiece == null && selectedTileObject != null)
                {
                    MovePiece(tile);
                }
                else if (tile.CurrentPiece != null && selectedTileObject != null)
                {
                    if (selectedTile.CurrentPiece.color == tile.CurrentPiece.color)
                    {
                        //selectedTileObject.SetDefaultColor();
                        //UnhighlightTiles(); // possibleTileMoves);
                        ChangeSelectedTile(tile.tileObject);
                        HighlightTiles(possibleTileMoves);
                        //selectedTileObject.SetSelectedColor();
                    }
                    else
                    {
                        
                        MovePiece(tile);
                    }
                }
                else if (selectedTileObject == null && tile.CurrentPiece != null)
                {
                    ChangeSelectedTile(tile.tileObject);
                    //selectedTileObject.SetSelectedColor();
                    HighlightTiles(possibleTileMoves);
                }
                else if (tile == selectedTile)
                {
                    //selectedTileObject.SetDefaultColor();
                    UnhighlightTiles(); //possibleTileMoves);
                    ChangeSelectedTile(null);
                }
            }
        }
    }

    private void MovePiece(Tile tile)
    {
        Profiler.BeginSample("MovePiece");
        if (selectedTileObject.tile.CurrentPiece.color != BoardCreator.playerColor)
        {
            return;
        }
        List<Tile> possibleMoves = selectedTile.CurrentPiece.GetPossibleMoves(false, true);

        for (int i = 0; i < possibleMoves.Count; i++)
        {
            if (possibleMoves[i].x == tile.x && possibleMoves[i].y == tile.y)
            {
                NewMethod();
                Piece piece = selectedTileObject.tile.CurrentPiece;   
                selectedTileObject.tile.CurrentPiece.MovePiece(tile.x, tile.y);
                ChangeSelectedTile(null);

                Buff b = BuffManager.Instance.FindBuff(piece, "doubleMove");
                if (b != null)
                {
                    doubleMovePiece = piece;
                    b.RemoveTokens(1);
                    return;
                }
                else
                {
                    //doubleMovePiece = null;
                }
                GameManager.instance.EndTurn();
                return;
            }
        }
        selectedTileObject.SetDefaultColor();
        UnhighlightTiles(); //possibleTileMoves);
        ChangeSelectedTile(null);
        Profiler.EndSample();

        void NewMethod()
        {
            UnhighlightTiles(); //possibleTileMoves);
            selectedTile.CurrentPiece.hasMoved = true;
            selectedTileObject.SetDefaultColor();
        }
    }
    
    private void SetSelectedPiece(Piece piece)
    {
        selectedPiece = piece;

        bool castableAbilities = piece.color != Colors.Black;

        UI.Instance.DisplayInfoContainer(piece, castableAbilities);
    }
    public void SetSelectedAbility(Ability ability, List<Tile> tiles = null)
    {
        selectedAbility = ability;
        if (ability != null)
        {
            UnhighlightTiles();
            ChangeSelectedTile(null);
            //if (ability.cooldown > 0 || ability.owner.mana < AbilityManager.Instance.FindAbility(ability.name).manaCost) return;
            if (!ability.canBeCasted()) return;
            foreach (Tile tile in tiles)
            {
                tile.tileObject.SetAbilityHightlightedColor();
                //tile.currentAbility = selectedAbility;
                abilityTiles = tiles;
                highlightedTiles.Add(tile.tileObject);
            }
        }
        else
        {
            abilityTiles.Clear();
        }
        //SetSelectedPiece(null);
        //UnhighlightTiles();
    }
    /*public void HighlightAbilityTiles(List<Tile> tiles)
    {

    }*/
    public void SetMovedTiles((Tile, Tile) tiles)
    {
        tiles.Item1.tileObject.GetComponent<MeshRenderer>().material.color = Color.red;
        tiles.Item2.tileObject.GetComponent<MeshRenderer>().material.color = Color.red;
        highlightedTiles.Add(tiles.Item1.tileObject);
        highlightedTiles.Add(tiles.Item2.tileObject);
    }
    private void HighlightTiles(List<Tile> tiles)
    {
        UnhighlightTiles();
        foreach (Tile tile in tiles)
        {
            tile.tileObject.SetHighlightedColor();
            highlightedTiles.Add(tile.tileObject);
        }
    }
    private void UnhighlightTiles() //(List<Tile> tiles)
    {
        foreach (TileObject tileObject in highlightedTiles)
        {
            tileObject.SetDefaultColor();
            tileObject.tile.currentAbility = null;
            //selectedPiece = null;
            //SetSelectedAbility(null);
        }
        highlightedTiles.Clear();
    }
    public void ChangeSelectedTile(TileObject tileObject)
    {
        if (selectedTileObject != null) selectedTileObject.SetDefaultColor();
        selectedTileObject = tileObject;
        selectedTile = tileObject?.tile;
        if (selectedTileObject != null)
        {
            selectedTileObject.SetSelectedColor();
        }
        if (selectedTileObject != null && selectedTile.CurrentPiece != null) possibleTileMoves = selectedTile.CurrentPiece.GetPossibleMoves(false, true);
        else possibleTileMoves.Clear();
    }
}
