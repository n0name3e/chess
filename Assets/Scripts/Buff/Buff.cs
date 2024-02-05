public class Buff
{
    public string name;
    public int maxDuration;
    public int duration;
    public bool isUnlimited = false;
    public int charges;
    public bool isCharges;
    public Colors turnAdded;

    public delegate void AddBuff(Piece piece);
    public AddBuff OnAddBuff;

    public delegate void AddTileBuff(Tile tile);
    public AddTileBuff OnTileAddBuff;

    public delegate void RemoveBuff(Piece piece);
    public RemoveBuff OnRemoveBuff;

    public delegate void TurnEndBuffTile(Tile tile, Colors colors);
    public TurnEndBuffTile OnTurnEndTile;

    public Buff(string name, int duration)
    {
        this.name = name;
        maxDuration = duration;
        this.duration = duration;
    }
    public void AddCharge(int charges)
    {
        this.charges += charges;
        isCharges = true;
    }
    public void RemoveCharges(int charges)
    {
        this.charges -= charges;
        BuffManager.Instance.CheckAllBuffs(Colors.White);
        BuffManager.Instance.CheckAllBuffs(Colors.Black);
    }
}
