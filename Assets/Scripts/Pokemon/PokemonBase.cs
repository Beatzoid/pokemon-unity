using System.Collections.Generic;
using UnityEngine;

public enum PokemonType
{
    None,
    Normal,
    Fire,
    Water,
    Electric,
    Grass,
    Ice,
    Fighting,
    Poison,
    Ground,
    Flying,
    Psychic,
    Bug,
    Rock,
    Ghost,
    Dragon,
    Dark,
    Steel,
    Fairy
}

public enum Stat
{
    Attack,
    Defense,
    SpecialAttack,
    SpecialDefense,
    Speed,

    // Not actual stats, used to boost moveAccuracy
    Accuracy,
    Evasion
}

/// <summary>
/// The PokemonBase class holds all data for all pokemon
/// </summary>
[CreateAssetMenu(fileName = "Pokemon", menuName = "Pokemon/Create new pokemon")]
public class PokemonBase : ScriptableObject
{
    [Header("Info")]
    [SerializeField] private string pokemonName;

    [TextArea]
    [SerializeField] private string description;

    [Space(5)]
    [Header("Sprites")]

    [SerializeField] private Sprite frontSprite;
    [SerializeField] private Sprite backSprite;

    [Space(5)]
    [Header("Types")]

    [SerializeField] private PokemonType type1;
    [SerializeField] private PokemonType type2;

    [Space(5)]
    [Header("Stats")]

    [SerializeField] private int maxHp;
    [SerializeField] private int attack;
    [SerializeField] private int defense;
    [SerializeField] private int specialAttack;
    [SerializeField] private int specialDefense;
    [SerializeField] private int speed;

    [Space(5)]
    [Header("Misc")]
    [SerializeField] private List<LearnableMove> learnableMoves;

    public string Name
    {
        get { return pokemonName; }
    }

    public string Description
    {
        get { return description; }
    }

    public Sprite FrontSprite
    {
        get { return frontSprite; }
    }

    public Sprite BackSprite
    {
        get { return backSprite; }
    }

    public PokemonType Type1
    {
        get { return type1; }
    }

    public PokemonType Type2
    {
        get { return type2; }
    }

    public int MaxHp
    {
        get { return maxHp; }
    }

    public int Attack
    {
        get { return attack; }
    }

    public int Defense
    {
        get { return defense; }
    }

    public int SpecialAttack
    {
        get { return specialAttack; }
    }

    public int SpecialDefense
    {
        get { return specialDefense; }
    }

    public int Speed
    {
        get { return speed; }
    }

    public List<LearnableMove> LearnableMoves
    {
        get { return learnableMoves; }
    }
}
