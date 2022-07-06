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
    Phychic,
    Bug,
    Rock,
    Ghost,
    Dragon
}

[CreateAssetMenu(fileName = "Pokemon", menuName = "Pokemon/Create new pokemon")]
public class PokemonBase : ScriptableObject
{
    [Header("Info")]
    [SerializeField] readonly string pokemonName;

    [TextArea]
    [SerializeField] readonly string description;

    [Space(5)]
    [Header("Sprites")]

    [SerializeField] readonly Sprite frontSprite;
    [SerializeField] readonly Sprite backSprite;

    [Space(5)]
    [Header("Types")]

    [SerializeField] readonly PokemonType type1;
    [SerializeField] readonly PokemonType type2;

    [Space(5)]
    [Header("Stats")]

    [SerializeField] readonly int maxHp;
    [SerializeField] readonly int attack;
    [SerializeField] readonly int defense;
    [SerializeField] readonly int specialAttack;
    [SerializeField] readonly int specialDefense;
    [SerializeField] readonly int speed;

    [Space(5)]
    [Header("Misc")]
    [SerializeField] readonly List<LearnableMove> learnableMoves;

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

    public PokemonType Typ2
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

