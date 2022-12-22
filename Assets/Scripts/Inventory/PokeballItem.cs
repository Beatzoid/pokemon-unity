using UnityEngine;

/// <summary>
/// The PokeballItem manages all pokeball-related logic
/// </summary>
[CreateAssetMenu(menuName = "Items/Create new pokeball")]

public class PokeballItem : ItemBase
{
    public override bool Use(Pokemon pokemon)
    {
        return true;
    }
}
