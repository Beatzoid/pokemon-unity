using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The MoveDB class manages all the moves for the pokemon
/// </summary>
public class MoveDB
{
    private static Dictionary<string, MoveBase> moves;

    /// <summary>
    /// Initialize the MoveDB class
    /// </summary>
    public static void Init()
    {
        moves = new();

        MoveBase[] moveList = Resources.LoadAll<MoveBase>("");

        foreach (MoveBase move in moveList)
        {
            if (moves.ContainsKey(move.MoveName))
            {
                Debug.LogError($"There are two moves with the name {move.MoveName}");
                continue;
            }

            moves[move.MoveName] = move;
        }
    }

    /// <summary>
    /// Get a move by it's name
    /// </summary>
    /// <param name="name">The name of the move to get </param>
    public static MoveBase GetMoveByName(string name)
    {
        if (!moves.ContainsKey(name))
        {
            Debug.LogError($"Move with name {name} was not found in the database");
            return null;
        }

        return moves[name];
    }
}
