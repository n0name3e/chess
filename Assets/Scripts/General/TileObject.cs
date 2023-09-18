using UnityEngine;

public class TileObject : MonoBehaviour
{
    public Tile tile;
    private Color color;
    public int x;
    public int y;
    private MeshRenderer meshRenderer;

    private void OnEnable()
    {
        tile = new Tile(x, y, this);
        meshRenderer = GetComponent<MeshRenderer>();
    }
    public void ChangeColor(Color c)
    {
        color = c;
        meshRenderer.material.color = color;
    }
    public void SetDefaultColor()
    {
        if ((x + y) % 2 == 0)
        {
            meshRenderer.material.color = BoardCreator.Instance.blackColor;
        }
        else
        {
            meshRenderer.material.color = BoardCreator.Instance.whiteColor;
        }
    }
    public void SetSelectedColor()
    {
        meshRenderer.material.color = BoardCreator.Instance.selectedColor;
    }
    public void SetHighlightedColor()
    {
        meshRenderer.material.color = BoardCreator.Instance.highlightedColor;
    }
    public void SetAbilityHightlightedColor()
    {
        if (tile.CurrentPiece == null)
        {
            meshRenderer.material.color = BoardCreator.Instance.abilityHighlighedColorFriend;
            return;
        }
        if (tile.CurrentPiece.color == Colors.Black)
        {
            meshRenderer.material.color = BoardCreator.Instance.abilityHighlighedColorEnemy;
        }
        else
        {
            meshRenderer.material.color = BoardCreator.Instance.abilityHighlighedColorFriend;
        }
    }
}
