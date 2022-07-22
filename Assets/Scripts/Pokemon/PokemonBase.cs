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

    [Range(0, 255)]
    [SerializeField] private int catchRate = 255;

    [Space(5)]
    [Header("Misc")]
    [SerializeField] private List<LearnableMove> learnableMoves;

    public string Name => pokemonName;

    public string Description => description;

    public Sprite FrontSprite => frontSprite;

    public Sprite BackSprite => backSprite;

    public PokemonType Type1 => type1;

    public PokemonType Type2 => type2;

    public int MaxHp => maxHp;

    public int Attack => attack;

    public int Defense => defense;

    public int SpecialAttack => specialAttack;

    public int SpecialDefense => specialDefense;

    public int Speed => speed;

    public List<LearnableMove> LearnableMoves => learnableMoves;

    public int CatchRate => catchRate;
}
