using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The MoveEffects class manages effects for pokemon moves
/// </summary>
[System.Serializable]
public class MoveEffects
{
    [SerializeField] private List<StatBoost> boosts;
    [SerializeField] private ConditionID status;
    [SerializeField] private ConditionID volatileStatus;

    public List<StatBoost> Boosts
    {
        get { return boosts; }
    }

    public ConditionID Status
    {
        get { return status; }
    }

    public ConditionID VolatileStatus
    {
        get { return volatileStatus; }
    }
}

/// <summary>
/// The SecondaryEffects class manages all secondary effects for pokemon moves
/// </summary>
[System.Serializable]
public class SecondaryEffects : MoveEffects
{
    [SerializeField] private int chance;
    [SerializeField] private MoveTarget target;

    public int Chance
    {
        get { return chance; }
    }

    public MoveTarget Target
    {
        get { return target; }
    }
}
