using UnityEngine;

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
	[SerializeField] private int pp;

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

	public int PP
	{
		get { return pp; }
	}

	public bool IsSpecial
	{
		get
		{
			if (type == PokemonType.Fire || type == PokemonType.Water || type == PokemonType.Grass
			|| type == PokemonType.Ice || type == PokemonType.Electric || type == PokemonType.Dragon)
			{
				return true;
			}
			else
			{
				return false;
			}
		}
	}
}
