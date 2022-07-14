using System.Collections.Generic;

public enum ConditionID
{
    none, posion, burn, sleep, paralyze, freeze
}

public class ConditionsDB
{
    public static Dictionary<ConditionID, Condition> Conditions { get; set; } = new Dictionary<ConditionID, Condition>()
    {
        {
            ConditionID.posion,
            new Condition()
            {
                Name="Poison",
                StartMessage="has been poisoned",
                OnAfterTurn = (Pokemon pokemon) =>
                {
                    pokemon.UpdateHP(pokemon.MaxHp / 8);
                    pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} hurt itself due to poison!");
                }
            }
        },
        {
        ConditionID.burn,
            new Condition()
            {
                Name="Burn",
                StartMessage="has been burned",
                OnAfterTurn = (Pokemon pokemon) =>
                {
                    pokemon.UpdateHP(pokemon.MaxHp / 16);
                    pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} burned!");
                }
            }
        }
    };
}
