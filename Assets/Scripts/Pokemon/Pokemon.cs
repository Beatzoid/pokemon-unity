using System.Collections.Generic;
using UnityEngine;

public class Pokemon
{
    public PokemonBase Base { get; set; }
    public int Level { get; set; }
    public int HP { get; set; }
    public List<Move> Moves { get; set; }

    public Pokemon(PokemonBase pBase, int pLevel)
    {
        Base = pBase;
        Level = pLevel;
        HP = MaxHp;

        Moves = new List<Move>();

        // Generate moves based on pokemon level
        foreach (LearnableMove move in Base.LearnableMoves)
        {
            if (move.Level <= Level)
                Moves.Add(new Move(move.MoveBase));

            if (Moves.Count >= 4)
                break;
        }
    }

    /// <summary>
    ///     Causes the specified pokemon to take damage
    /// </summary>
    /// <param name="move">The move to apply to the pokemon </param>
    /// <param name="attacker">The attacking pokemon </param>
    /// <returns>
    /// A boolean representing whether the pokemon fainted
    /// </returns>
    public bool TakeDamage(Move move, Pokemon attacker)
    {
        float modifiers = Random.Range(0.85f, 1f);
        float a = (2 * attacker.Level + 10) / 250f;

        float d = a * move.Base.Power * ((float)attacker.Attack / Defense) + 2;
        int damage = Mathf.FloorToInt(d * modifiers);

        HP -= damage;
        if (HP <= 0)
        {
            HP = 0;
            return true;
        }

        return false;
    }

    public Move GetRandomMove()
    {
        int r = Random.Range(0, Moves.Count);
        return Moves[r];
    }

    public int Attack
    {
        get { return Mathf.FloorToInt(Base.Attack * Level / 100f) + 5; }
    }

    public int Defense
    {
        get { return Mathf.FloorToInt(Base.Defense * Level / 100f) + 5; }
    }

    public int SpecialAttack
    {
        get { return Mathf.FloorToInt(Base.SpecialAttack * Level / 100f) + 5; }
    }

    public int SpecialDefense
    {
        get { return Mathf.FloorToInt(Base.SpecialAttack * Level / 100f) + 5; }
    }

    public int Speed
    {
        get { return Mathf.FloorToInt(Base.Speed * Level / 100f) + 5; }
    }

    public int MaxHp
    {
        get { return Mathf.FloorToInt(Base.Speed * Level / 100f) + 10; }
    }
}
