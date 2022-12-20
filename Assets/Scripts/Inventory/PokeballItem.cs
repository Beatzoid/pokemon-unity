using UnityEngine;

[CreateAssetMenu(menuName = "Items/Create new pokeball")]

public class PokeballItem : ItemBase
{
    public override bool Use(Pokemon pokemon)
    {
        return true;
    }
}
