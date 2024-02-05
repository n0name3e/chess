using System.Collections.Generic;
using UnityEngine;

public class BuffManager : MonoBehaviour
{
    private static BuffManager instance;

    public static BuffManager Instance { get => instance; set => instance = value; }

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }
    public void AddBuff(Piece pieceToAdd, Buff buff)
    {
        Buff b = FindBuff(pieceToAdd, buff.name);
        if (b != null)
        {
            b.duration = b.maxDuration;
            return;
        }
        pieceToAdd.buffs.Add(buff);
        buff.OnAddBuff?.Invoke(pieceToAdd);
    }
    public void AddBuff(Tile tileToAdd, Buff buff)
    {
        Buff b = FindBuff(tileToAdd, buff.name);
        if (b != null)
        {
            b.duration = b.maxDuration;
            return;
        }
        tileToAdd.buffs.Add(buff);
        buff.OnTileAddBuff?.Invoke(tileToAdd);
        if (buff.OnTurnEndTile != null)
        { // todo
            //GameManager.instance.Delegates.OnEndTurn += buff.OnTurnEndTile;
        }
    }
    private void UpdateBuffsOnTurn(Colors currentTurnColor)
    {
        foreach (Tile tile in BoardCreator.mainBoard.tiles)
        {
            foreach (Buff buff in tile.buffs)
            {
                buff.OnTurnEndTile.Invoke(tile, currentTurnColor);
            }
        }
    }

    public void UpdateAllBuff(Colors currentTurnColor)
    {
        foreach (Piece piece in new List<Piece>(BoardCreator.mainBoard.pieces))
        {
            if (piece.color != currentTurnColor) continue;
            foreach (Buff buff in new List<Buff>(piece.buffs))
            {
                if (buff.isUnlimited) continue;
                buff.duration--;
                if (buff.duration <= 0)
                {
                    RemoveBuff(piece, buff);
                }
            }
        }
        foreach (Tile tile in new List<Tile>(BoardCreator.mainBoard.tiles))
        {
            foreach (Buff buff in new List<Buff>(tile.buffs))
            {
                if (currentTurnColor != buff.turnAdded) continue;
                if (buff.isUnlimited) continue;
                buff.duration--;
                if (buff.duration <= 0)
                {
                    print("duration");
                    RemoveBuff(tile, buff);
                }
            }
        }
        UpdateBuffsOnTurn(currentTurnColor);
    }
    public void CheckAllBuffs(Colors color)
    {
        foreach (Piece piece in new List<Piece>(BoardCreator.mainBoard.pieces))
        {
            foreach (Buff buff in new List<Buff>(piece.buffs))
            {
                if (buff.isCharges && buff.charges <= 0)
                {
                    RemoveBuff(piece, buff);
                }
            }
        }
    }
    public void RemoveBuff(Piece piece, Buff buff)
    {
        buff.OnRemoveBuff?.Invoke(piece);
        piece.buffs.Remove(buff);
    }
    public void RemoveBuff(Tile tile, Buff buff)
    {
        tile.buffs.Remove(buff);
    }
    public Buff FindBuff(Tile tile, string buff)
    {
        if (tile == null) return null;
        foreach (Buff b in tile.buffs)
        {
            if (b.name == buff) return b;
        }
        return null;
    }
    public Buff FindBuff(Piece piece, string buff)
    {
        foreach (Buff b in piece.buffs)
        {
            if (b.name == buff) return b;
        }
        return null;
    }
}
