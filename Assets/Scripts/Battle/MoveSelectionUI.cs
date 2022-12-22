using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

/// <summary>
/// The MoveSelectionUI class manages the move selection UI in the battle scene
/// </summary>
public class MoveSelectionUI : MonoBehaviour
{
    [SerializeField] private List<TextMeshProUGUI> moveTexts;

    private int currentSelection = 0;

    /// <summary>
    /// Set the move data for the move selection UI
    /// </summary>
    /// <param name="currentMoves"> The list of current moves </param>
    /// <param name="newMove"> The new move to add </param>
    public void SetMoveData(List<MoveBase> currentMoves, MoveBase newMove)
    {
        for (int i = 0; i < currentMoves.Count; ++i)
        {
            moveTexts[i].text = currentMoves[i].MoveName;
        }

        moveTexts[currentMoves.Count].text = newMove.MoveName;
    }

    /// <summary>
    /// Handles the move selection logic
    /// </summary>
    /// <param name="onSelected"> The action to call when the player selects an action </param>
    public void HandleMoveSelection(Action<int> onSelected)
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
            ++currentSelection;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            --currentSelection;

        currentSelection = Mathf.Clamp(currentSelection, 0, PokemonBase.MaxNumOfMoves);
        UpdateMoveSelection(currentSelection);

        if (Input.GetKeyDown(KeyCode.Return))
            onSelected?.Invoke(currentSelection);
    }

    /// <summary>
    /// Update the move selection UI
    /// </summary>
    /// <param name="selection"> The index of the current selection <param>
    public void UpdateMoveSelection(int selection)
    {
        for (int i = 0; i < PokemonBase.MaxNumOfMoves + 1; ++i)
        {
            if (i == selection)
                moveTexts[i].color = GlobalSettings.Instance.HighlightedColor;
            else
                moveTexts[i].color = Color.black;
        }
    }
}
