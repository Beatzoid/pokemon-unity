using UnityEngine;

/// <summary>
/// The LearnableMove class manages moves that can be unlocked
/// when the pokemon reaches a certain level
/// </summary>
[System.Serializable]
public class LearnableMove
{
    [SerializeField] private MoveBase moveBase;
    [SerializeField] private int level;

    public MoveBase MoveBase
    {
        get { return moveBase; }
    }

    public int Level
    {
        get { return level; }
    }
}
