using UnityEngine;

/// <summary>
/// THe TMITem class manages all the TM items in the game.
/// TM items are used to teach pokemon new moves
/// </summary>
[CreateAssetMenu(menuName = "Items/Create new TM or HM")]
public class TMItem : ItemBase
{
    [SerializeField] private MoveBase move;
    [SerializeField] private bool isHM;

    public MoveBase Move => move;
    public bool IsHM => isHM;

    public override string Name => $"{base.Name}: {move.MoveName}";
    public override string Description => $"Teaches the pokemon {move.MoveName}";

    public override bool CanUseInBattle => false;
    public override bool IsReusable => isHM;

    public override bool Use(Pokemon pokemon)
    {
        // Learning the move is handled from the InventoryUI class
        // If it was successfully learned, then return true
        return pokemon.HasMove(move);
    }

    public bool CanBeTaught(Pokemon pokemon)
    {
        return pokemon.Base.LearnableByItems.Contains(move);
    }
}
