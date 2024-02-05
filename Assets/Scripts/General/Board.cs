using System.Collections.Generic;

public class Board
{
    public List<Piece> pieces = new List<Piece>();
    public List<Tile> tiles = new List<Tile>();
    public string name = "Not main";
    public void CopyPieces(List<Piece> piecess)
    {
        foreach (Piece piece in new List<Piece>(piecess))
        {
            Piece clonedPiece = new Piece(piece, this);
            pieces.Add(clonedPiece);
            clonedPiece.Stun(piece.stunTimer);
            CopyAbilities(piece, clonedPiece);
        }
    }
    private void CopyAbilities(Piece piece, Piece clonePiece)
    {
        foreach (Ability ability in piece.abilities)
        {
            Ability a = new Ability(ability.name, clonePiece);
            a.charges = ability.charges;
            a.activated = ability.activated;
            a.cooldown = ability.cooldown;
            a.CopyDelegates(ability);
            a.owner = clonePiece;
            clonePiece.abilities.Add(a);
        }
    }
    public Tile FindTile(int x, int y)
    {
        if (x < 1 || y < 1 || x > 8 || y > 8) return null;
        if ((x - 1) * 8 + (y - 1) >= 0 && (x - 1) * 8 + (y - 1) > 63) // tile doesnt exist
        {
            return null;
        }
        return tiles[(x - 1) * 8 + (y - 1)];
    }
    /*public Tile FuckTile(int x, int y)
    {
        if (x < 1 || y < 1 || x > 8 || y > 8) return null;

        for (int i = 0; i < tiles.Count; i++)
        {
            if (x == tiles[i].x && y == tiles[i].y)
            {
                return tiles[i];
            }
        }
        return null;
    }*/
    public void DestroyBoard()
    {
        for (int i = 0; i < pieces.Count; i++)
        {
            pieces[i].ClearDelegates();
        }
        pieces.Clear();
        
    }
    public Board()
    {
        if (BoardCreator.mainBoard == null) return;
        /*Tile[] tiles = BoardCreator.mainBoard.tiles;
        int lenght = tiles.Length;
        for (int i = 0; i < lenght; i++)
        {
            Tile clonedTile = new Tile(tiles[i]);
            tiles[i] = clonedTile;
            clonedTile.board = this;
        }*/

        foreach (Tile tile in BoardCreator.mainBoard.tiles)
        {
            Tile clonedTile = new Tile(tile);
            tiles.Add(clonedTile);
            clonedTile.board = this;
        }

    }
}
