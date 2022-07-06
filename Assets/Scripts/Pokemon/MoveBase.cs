using UnityEngine;

[CreateAssetMenu(fileName = "Move", menuName = "Pokemon/Create new move")]
public class MoveBase : ScriptableObject
{
    [Header("Info")]
    [SerializeField] readonly string moveName;

    [TextArea]
    [SerializeField] readonly string description;

    [Space(5)]
    [Header("Stats")]

    [SerializeField] readonly PokemonType type;
    [SerializeField] readonly int power;
    [SerializeField] readonly int accuracy;
    [SerializeField] readonly int pp;

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
}
