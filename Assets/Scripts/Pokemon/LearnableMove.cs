using UnityEngine;

[System.Serializable]
public class LearnableMove
{
    [SerializeField] readonly MoveBase moveBase;
    [SerializeField] readonly int level;

    public MoveBase MoveBase
    {
        get { return moveBase; }
    }

    public int Level
    {
        get { return level; }
    }
}
