using System.Collections.Generic;
using UnityEngine;

public enum MoveCategory
{
    Physical, Special, Status
}

public enum MoveTarget
{
    enemy, self
}

/// <summary>
/// The MoveBase class holds all data for a move
/// </summary>
[CreateAssetMenu(fileName = "Move", menuName = "Pokemon/Create new move")]
public class MoveBase : ScriptableObject
{
    [Header("Info")]
    [SerializeField] private string moveName;

    [TextArea]
    [SerializeField] private string description;

    [Space(5)]
    [Header("Stats")]

    [SerializeField] private PokemonType type;
    [SerializeField] private int power;
    [SerializeField] private int accuracy;
    [SerializeField] private bool alwaysHits;
    [SerializeField] private int pp;
    [SerializeField] private int priority;
    [SerializeField] private MoveCategory category;
    [SerializeField] private MoveTarget target;

    [Space(5)]
    [Header("Effects")]
    [Space(2)]
    [SerializeField] private MoveEffects effects;
    [SerializeField] private List<SecondaryEffects> secondaryEffects;

    public string MoveName
    {
        get { return moveName; }
    }

    public string Description
    {
        get { return description; }
    }

    public PokemonType Type
    {
        get { return type; }
    }

    public int Power
    {
        get { return power; }
    }

    public int Accuracy
    {
        get { return accuracy; }
    }

    public bool AlwaysHits
    {
        get { return alwaysHits; }
    }

    public int PP
    {
        get { return pp; }
    }

    public int Priority
    {
        get { return priority; }
    }

    public MoveCategory Category
    {
        get { return category; }
    }

    public MoveEffects Effects
    {
        get { return effects; }
    }

    public List<SecondaryEffects> SecondaryEffects
    {
        get { return secondaryEffects; }
    }

    public MoveTarget Target
    {
        get { return target; }
    }
}
