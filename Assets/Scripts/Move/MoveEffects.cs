using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The MoveEffects class manages effects for pokemon moves
/// </summary>
[System.Serializable]
public class MoveEffects
{
    [SerializeField] private List<StatBoost> boosts;

    public List<StatBoost> Boosts
    {
        get { return boosts; }
    }
}
