using UnityEngine;

[CreateAssetMenu(fileName = "Move", menuName = "Pokemon/Create new move")]
public class MoveBase : ScriptableObject
{
    [Header("Info")]
    [SerializeField] string moveName;

    [TextArea]
    [SerializeField] string description;

    [Space(5)]
    [Header("Stats")]

    [SerializeField] PokemonType type;
    [SerializeField] int power;
    [SerializeField] int accuracy;
    [SerializeField] int pp;

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
