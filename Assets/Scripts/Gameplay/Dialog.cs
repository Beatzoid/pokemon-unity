
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The Dialog class holds the data for the dialogs in the game
/// </summary>
[System.Serializable]
public class Dialog
{
    [TextArea(1, 2)]
    [SerializeField] private List<string> lines;

    public List<string> Lines
    {
        get { return lines; }
    }
}
