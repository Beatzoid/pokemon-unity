using System.Collections.Generic;
using UnityEngine;

public enum ConditionID
{
    none, poison, burn, sleep, paralyze, freeze
}

public class ConditionsDB
{
    public static Dictionary<ConditionID, Condition> Conditions { get; set; } = new Dictionary<ConditionID, Condition>()
    {
        {
            ConditionID.poison,
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
        },
        {
        ConditionID.paralyze,
            new Condition()
            {
                Name="Paralyze",
                StartMessage="has been paralyzed",
                OnBeforeMove = (Pokemon pokemon) =>
                {
                    if(Random.Range(1,5 ) == 1)
                    {
                        pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name}'s paralyzed!");
                        return false;
                    }

                    return true;
                }
            }
        },
        {
        ConditionID.freeze,
            new Condition()
            {
                Name="Freeze",
                StartMessage="has been frozen",
                OnBeforeMove = (Pokemon pokemon) =>
                {
                    if(Random.Range(1,5 ) == 1)
                    {
                        pokemon.CureStatus();
                        pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} unfroze itself!");
                        return true;
                    }

                    return false;
                }
            }
        },
         {
        ConditionID.sleep,
            new Condition()
            {
                Name="Sleep",
                StartMessage="has fallen asleep",
                OnStart = (Pokemon pokemon) =>
                {
                    // Sleep for 1-3 turns
                    pokemon.StatusTime = Random.Range(1, 4);
                    Debug.Log($"Will be asleep for {pokemon.StatusTime} moves");
                },
                OnBeforeMove = (Pokemon pokemon) =>
                {
                    if (pokemon.StatusTime <= 0)
                    {
                        pokemon.CureStatus();
                        pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} woke up");
                        return true;
                    }

                    pokemon.StatusTime--;
                    pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} is sleeping");
                    return false;
                }
            }
        }
    };
}
