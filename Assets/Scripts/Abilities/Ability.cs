public class Ability
{
    public string name;
    public int cooldown;
    public float manaCost;
    public bool general = false;

    /// <summary>
    /// can not be activated and have passive effect
    /// </summary>
    public bool passive = false;
    public Piece owner;
    private bool isCharges;

    /// <summary>
    /// can be used if true and need conditions to activate if false!
    /// </summary>
    public bool activated = true;
    public int charges;
    public Ability(string name, Piece owner)
    {
        this.name = name;
        this.owner = owner;
        if (owner == null) general = true;
        abilityData = AbilityManager.Instance.FindAbility(name);
        manaCost = abilityData.manaCost;
    }
    public bool canBeCasted()
    {
        if (!activated) return false;
        isCharges = AbilityManager.Instance.FindAbility(name).chargesConditions.Count > 0;
        if (isCharges && charges <= 0) return false;
        if (cooldown > 0 || (!general && owner.mana < manaCost))
        {
            return false;
        }
        return true;
    }
    public void CopyDelegates(Ability originalAbility)
    {
        OnTargetedAbilityChoose = originalAbility.OnTargetedAbilityChoose;
        OnUntargetedAbilityChoose = originalAbility.OnUntargetedAbilityChoose;
        OnTileChoose = originalAbility.OnTileChoose;
        OnStart = originalAbility.OnStart;
        OnBotChoose = originalAbility.OnBotChoose;
    }
    public AbilityData abilityData;

    public delegate System.Collections.Generic.List<Tile> TargetedAbilityChoose(Piece piece);
    public TargetedAbilityChoose OnTargetedAbilityChoose;

    public delegate bool UntargetedAbilityChoose(Piece piece);
    public UntargetedAbilityChoose OnUntargetedAbilityChoose;

    public delegate void TileChoose(Tile tile, Piece castingPiece);
    public TileChoose OnTileChoose;

    public delegate (float, Tile) BotMove(Board board, Ability ability);
    public BotMove OnBotChoose;

    public delegate void Init(Piece piece);
    public Init OnStart;

    // for general ability
    public delegate System.Collections.Generic.List<Tile> GeneralAbilityChoose(Board board);
    public GeneralAbilityChoose OnGeneralAbilityChoose;
    
    public delegate void GeneralAbilityTileChoose(Tile tile);
    public GeneralAbilityTileChoose OnGeneralAbilityTileChoose;
}
