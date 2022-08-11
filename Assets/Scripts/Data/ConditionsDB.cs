using System.Collections.Generic;
using UnityEngine;

public enum ConditionID
{
    none, PSN, BRN, SLP, PRZ, FRZ, confusion
}

public class ConditionsDB
{
    /// <summary>
    /// Setup the Conditions DB
    /// </summary>
    public static void Init()
    {
        foreach (KeyValuePair<ConditionID, Condition> kvp in Conditions)
        {
            ConditionID conditionId = kvp.Key;
            Condition condition = kvp.Value;

            condition.Id = conditionId;
        }
    }

    public static Dictionary<ConditionID, Condition> Conditions { get; set; } = new Dictionary<ConditionID, Condition>()
    {
        {
            ConditionID.PSN,
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
        ConditionID.BRN,
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
        ConditionID.PRZ,
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
        ConditionID.FRZ,
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
        ConditionID.SLP,
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
        },
        {
        ConditionID.confusion,
            new Condition()
            {
                Name="Confusion",
                StartMessage="has been confused",
                OnStart = (Pokemon pokemon) =>
                {
                    // Confusion for 1-4 turns
                    pokemon.VolatileStatusTime = Random.Range(1, 5);
                    Debug.Log($"Will be confused for {pokemon.VolatileStatusTime} moves");
                },
                OnBeforeMove = (Pokemon pokemon) =>
                {
                    if (pokemon.VolatileStatusTime <= 0)
                    {
                        pokemon.CureVolatileStatus();
                        pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} kicked out of confusion");
                        return true;
                    }
                    pokemon.VolatileStatusTime--;

                    // 50% chance to do move
                    if (Random.Range(1, 3) ==1 )
                        return true;

                    // Hurt by confusion
                    pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} is confused");
                    pokemon.UpdateHP(pokemon.MaxHp / 8);
                    pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} hurt itself due to confusion!");
                    return false;
                }
            }
        }
    };

    public static float GetStatusBonus(Condition condition)
    {
        if (condition == null)
            return 1f;
        else if (condition.Id == ConditionID.SLP || condition.Id == ConditionID.FRZ)
            return 2f;
        else if (condition.Id == ConditionID.PRZ || condition.Id == ConditionID.PSN || condition.Id == ConditionID.BRN)
            return 1.5f;

        return 1f;
    }
}
