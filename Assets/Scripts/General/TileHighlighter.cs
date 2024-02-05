using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileHighlighter : MonoBehaviour
{
    public List<TileObject> highlightedTiles = new List<TileObject>();
    public List<TileObject> abilityHighlitedTiles = new List<TileObject>();

    public static TileHighlighter Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void SetAbilityMovedTiles((Tile startingTile, Tile movedTile) tiles)
    {
        tiles.startingTile.tileObject.GetComponent<MeshRenderer>().material.color = Color.red;
        tiles.movedTile.tileObject.GetComponent<MeshRenderer>().material.color = Color.red;
        highlightedTiles.Add(tiles.startingTile.tileObject);
        highlightedTiles.Add(tiles.movedTile.tileObject);
    }
    public void HighlightTiles(List<Tile> tiles)
    {
        UnhighlightTiles();
        foreach (Tile tile in tiles)
        {
            tile.tileObject.SetHighlightedColor();
            highlightedTiles.Add(tile.tileObject);
        }
    }
    public void HighlightTile(TileObject tileObject, Color color)
    {
        tileObject.GetComponent<MeshRenderer>().material.color = color;
        if (!abilityHighlitedTiles.Contains(tileObject)) abilityHighlitedTiles.Add(tileObject);
    }
    public void UnhighlightTile(TileObject tile)
    {
        tile.SetDefaultColor();
        abilityHighlitedTiles.Remove(tile);
    }
    public void UnhighlightTiles()
    {
        foreach (TileObject tileObject in highlightedTiles)
        {
            tileObject.SetDefaultColor();
        }
        highlightedTiles.Clear();
        for (int i = 0; i < abilityHighlitedTiles.Count; i++)
        {
            abilityHighlitedTiles[i].GetComponent<MeshRenderer>().material.color = Color.blue;
        }
    }
}
