using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Pokemon
{
	[SerializeField] private PokemonBase _base;
	[SerializeField] private int level;

	public PokemonBase Base { get { return _base; } }
	public int Level { get { return level; } }
	public int HP { get; set; }
	public List<Move> Moves { get; set; }

	/// <summary>
	/// Initializes the Pokemon
	/// </summary>
	public void Init()
	{
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
	/// A <see cref="DamageDetails"> DamageDetails </see> class containing the effectiveness of the attack, whether it was a critical,
	/// and whether or not the pokemon fainted
	/// </returns>
	public DamageDetails TakeDamage(Move move, Pokemon attacker)
	{
		float critical = 1;

		if (Random.value * 100f <= 6.25) critical = 2;

		float effectiveness = TypeChart.GetEffectiveness(move.Base.Type, Base.Type1) * TypeChart.GetEffectiveness(move.Base.Type, Base.Type2);

		DamageDetails damageDetails = new()
		{
			TypeEffectiveness = effectiveness,
			Critical = critical,
			Fainted = false
		};

		float attack = move.Base.IsSpecial ? attacker.SpecialAttack : attacker.Attack;
		float defense = move.Base.IsSpecial ? attacker.SpecialDefense : attacker.Defense;

		float modifiers = Random.Range(0.85f, 1f) * effectiveness * critical;
		float a = ((2 * attacker.Level) + 10) / 250f;

		float d = (a * move.Base.Power * ((float)attack / defense)) + 2;
		int damage = Mathf.FloorToInt(d * modifiers);

		HP -= damage;
		if (HP <= 0)
		{
			HP = 0;
			damageDetails.Fainted = true;
		}

		return damageDetails;
	}

	/// <summary>
	/// Get a random move from the pokemon's move list
	/// </summary>
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
