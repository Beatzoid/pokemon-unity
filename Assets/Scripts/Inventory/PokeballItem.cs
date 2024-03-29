using UnityEngine;

/// <summary>
/// The PokeballItem manages all pokeball-related logic
/// </summary>
[CreateAssetMenu(menuName = "Items/Create new pokeball")]

public class PokeballItem : ItemBase
{
    [SerializeField] private float catchRateModifier = 1f;

    public float CatchRateModifier => catchRateModifier;

    public override bool CanUseOutsideBattle => false;

    public override bool Use(Pokemon pokemon)
    {
        return true;
    }
}
