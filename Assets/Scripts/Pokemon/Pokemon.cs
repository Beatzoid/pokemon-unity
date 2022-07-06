using System.Collections.Generic;
using UnityEngine;

public class Pokemon
{
    public int HP { get; set; }
    public List<Move> Moves { get; set; }

    readonly PokemonBase _base;
    readonly int level;

    public Pokemon(PokemonBase pBase, int pLevel)
    {
        _base = pBase;
        level = pLevel;
        HP = _base.MaxHp;

        Moves = new List<Move>();

        // Generate moves based on pokemon level
        foreach (LearnableMove move in _base.LearnableMoves)
        {
            if (move.Level <= level)
                Moves.Add(new Move(move.MoveBase));

            if (Moves.Count >= 4)
                break;
        }
    }

    public int Attack
    {
        get { return Mathf.FloorToInt(_base.Attack * level / 100f) + 5; }
    }

    public int Defense
    {
        get { return Mathf.FloorToInt(_base.Defense * level / 100f) + 5; }
    }

    public int SpecialAttack
    {
        get { return Mathf.FloorToInt(_base.SpecialAttack * level / 100f) + 5; }
    }

    public int SpecialDefense
    {
        get { return Mathf.FloorToInt(_base.SpecialAttack * level / 100f) + 5; }
    }

    public int Speed
    {
        get { return Mathf.FloorToInt(_base.Speed * level / 100f) + 5; }
    }

    public int MaxHp
    {
        get { return Mathf.FloorToInt(_base.Speed * level / 100f) + 10; }
    }
}
