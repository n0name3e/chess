using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

public class TileSelector : MonoBehaviour
{
    private static TileSelector _instance;

    private Camera _cam;
    private TileObject _selectedTileObject;

    public Piece SelectedPiece;
    private Ability _selectedAbility;

    private Tile _selectedTile;
    List<Tile> possibleTileMoves = new List<Tile>();

    private TileHighlighter _tileHighlighter;

    public Piece DoubleMovePiece;
    [SerializeField] private UnityEngine.UI.Text _text;


    private List<Tile> _abilityTiles = new List<Tile>();

    public static TileSelector Instance { get => _instance; set => _instance = value; }

    private void Awake()
    {
        if (_instance == null) _instance = this;
        else Destroy(gameObject);

        _tileHighlighter = TileHighlighter.Instance;
    }
    // Start is called before the first frame update
    void Start()
    {
        _cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.GameEnded) return;
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            print("touch");
            CheckTile();
            ShowStats();
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
        if (!GameManager.Instance.showAbilities) return;
        _selectedAbility = null;
        Ray ray;
        if (Input.touchCount > 0)
        {
            ray = _cam.ScreenPointToRay(Input.GetTouch(0).position);
        }
        else
        {
            ray = _cam.ScreenPointToRay(Input.mousePosition);
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
        if (DoubleMovePiece != null && BuffManager.Instance.FindBuff(DoubleMovePiece, "doubleMove") == null) DoubleMovePiece = null;
        Ray ray;
        if (Input.touchCount > 0)
        {
            ray = _cam.ScreenPointToRay(Input.GetTouch(0).position);
        }       
        else
        {
            ray = _cam.ScreenPointToRay(Input.mousePosition);
        }
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            TileObject tileObject = hit.transform.GetComponent<TileObject>();
            if (tileObject != null)
            {
                Tile tile = tileObject.tile;
                if (_abilityTiles.Contains(tile))
                {
                    AbilityManager.Instance.CastAbility(_selectedAbility, tile, SelectedPiece);
                    SetSelectedAbility(null);
                    _tileHighlighter.UnhighlightTiles();
                    return;
                }
                if (_selectedAbility != null)
                {
                    SetSelectedAbility(null);
                    SelectedPiece = null;
                    _tileHighlighter.UnhighlightTiles();
                }
                if (DoubleMovePiece != null && tileObject.tile.CurrentPiece != null
                    && tileObject.tile.CurrentPiece != DoubleMovePiece)
                { 
                    return;
                }
                if (_selectedTileObject == null && tile.CurrentPiece != null && tile.CurrentPiece.color != BoardCreator.playerColor)
                    return;
                if (tile.CurrentPiece == null && _selectedTileObject != null)
                {
                    MovePiece(tile);
                }
                else if (tile.CurrentPiece != null && _selectedTileObject != null)
                {
                    if (_selectedTile.CurrentPiece.color == tile.CurrentPiece.color)
                    {
                        ChangeSelectedTile(tile.tileObject);
                        _tileHighlighter.HighlightTiles(possibleTileMoves);
                    }
                    else
                    {                      
                        MovePiece(tile);
                    }
                }
                else if (_selectedTileObject == null && tile.CurrentPiece != null)
                {
                    ChangeSelectedTile(tile.tileObject);
                    _tileHighlighter.HighlightTiles(possibleTileMoves);
                }
                else if (tile == _selectedTile)
                {
                    _tileHighlighter.UnhighlightTiles();
                    ChangeSelectedTile(null);
                }
            }
        }
    }

    private void MovePiece(Tile tile)
    {
        Profiler.BeginSample("MovePiece");
        if (_selectedTileObject.tile.CurrentPiece.color != BoardCreator.playerColor)
        {
            return;
        }
        List<Tile> possibleMoves = _selectedTile.CurrentPiece.GetPossibleMoves(false, true);

        for (int i = 0; i < possibleMoves.Count; i++)
        {
            if (possibleMoves[i].x == tile.x && possibleMoves[i].y == tile.y)
            {
                NewMethod();
                Piece piece = _selectedTileObject.tile.CurrentPiece;   
                _selectedTileObject.tile.CurrentPiece.MovePiece(tile.x, tile.y);
                ChangeSelectedTile(null);

                Buff b = BuffManager.Instance.FindBuff(piece, "doubleMove");
                if (b != null)
                {
                    DoubleMovePiece = piece;
                    b.RemoveCharges(1);
                    return;
                }
                GameManager.Instance.EndTurn();
                return;
            }
        }
        _selectedTileObject.SetDefaultColor();
        _tileHighlighter.UnhighlightTiles(); 
        ChangeSelectedTile(null);
        Profiler.EndSample();

        void NewMethod()
        {
            _tileHighlighter.UnhighlightTiles(); 
            _selectedTile.CurrentPiece.hasMoved = true;
            _selectedTileObject.SetDefaultColor();
        }
    }
    
    private void SetSelectedPiece(Piece piece)
    {
        SelectedPiece = piece;

        bool castableAbilities = piece.color != Colors.Black;

        UI.Instance.DisplayInfoContainer(piece, castableAbilities);
    }
    public void SetSelectedAbility(Ability ability, List<Tile> tiles = null)
    {
        _selectedAbility = ability;
        if (ability != null)
        {
            _tileHighlighter.UnhighlightTiles();
            ChangeSelectedTile(null);
            if (!ability.canBeCasted()) return;
            foreach (Tile tile in tiles)
            {
                tile.tileObject.SetAbilityHightlightedColor();
                _abilityTiles = tiles;
                _tileHighlighter.highlightedTiles.Add(tile.tileObject);
            }
        }
        else
        {
            _abilityTiles.Clear();
        }
    }

    public void ChangeSelectedTile(TileObject tileObject)
    {
        if (_selectedTileObject != null) _selectedTileObject.SetDefaultColor();
        _selectedTileObject = tileObject;
        _selectedTile = tileObject?.tile;
        if (_selectedTileObject != null)
        {
            _selectedTileObject.SetSelectedColor();
        }
        if (_selectedTileObject != null && _selectedTile.CurrentPiece != null) possibleTileMoves = _selectedTile.CurrentPiece.GetPossibleMoves(false, true);
        else possibleTileMoves.Clear();
    }
}
