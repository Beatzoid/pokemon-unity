
using System.Collections.Generic;
using UnityEngine;

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
