using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BattleDialogBox : MonoBehaviour
{
    [SerializeField] readonly int lettersPerSecond;
    [SerializeField] Color highlightedColor;
    [SerializeField] readonly TextMeshProUGUI dialogText;
    [SerializeField] readonly GameObject actionSelector;
    [SerializeField] readonly GameObject moveSelector;
    [SerializeField] readonly GameObject moveDetails;

    [SerializeField] readonly List<TextMeshProUGUI> actionTexts;
    [SerializeField] readonly List<TextMeshProUGUI> moveTexts;

    [SerializeField] readonly TextMeshProUGUI ppText;
    [SerializeField] readonly TextMeshProUGUI typeText;

    /// <summary>
    /// Set the dialog box text
    /// </summary>
    /// <param name="dialog">The dialog text to set </param>
    public void SetDialog(string dialog)
    {
        dialogText.text = dialog;
    }

    /// <summary>
    /// Smoothly animate the dialog on the dialog box
    /// </summary>
    /// <param name="dialog">The dialog to animate </param>
    public IEnumerator TypeDialog(string dialog)
    {
        dialogText.text = "";

        foreach (char letter in dialog.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(1f / lettersPerSecond);
        }
    }

    /// <summary>
    /// Show/hide the dialog text
    /// </summary>
    /// <param name="enabled">Whether to show or hide the dialog text </param> 
    public void SetDialogTextActive(bool enabled)
    {
        dialogText.enabled = enabled;
    }

    /// <summary>
    /// Show/hide the action selector
    /// </summary>
    /// <param name="enabled">Whether to show or hide the action selector </param> 
    public void EnableActionSelector(bool enabled)
    {
        actionSelector.SetActive(enabled);
    }

    /// <summary>
    /// Show/hide the move selector and details
    /// </summary>
    /// <param name="enabled">Whether to show or hide the move selector amd details </param> 
    public void EnableMoveSelector(bool enabled)
    {
        moveSelector.SetActive(enabled);
        moveDetails.SetActive(enabled);
    }

    /// <summary>
    /// Update the action selection UI with the selected action
    /// </summary>
    /// <param name="selectedAction">The selected action </param> 
    public void UpdateActionSelection(int selectedAction)
    {
        for (int i = 0; i < actionTexts.Count; ++i)
        {
            if (i == selectedAction)
                actionTexts[i].color = highlightedColor;
            else
                actionTexts[i].color = Color.black;
        }
    }

    /// <summary>
    /// Update the move selection UI with the selected move
    /// </summary>
    /// <param name="selectedMove">The index of the selected move </param> 
    /// <param name="move">The selected move </param> 
    public void UpdateMoveSelection(int selectedMove, Move move)
    {
        for (int i = 0; i < moveTexts.Count; ++i)
        {
            if (i == selectedMove)
                moveTexts[i].color = highlightedColor;
            else
                moveTexts[i].color = Color.black;
        }

        ppText.text = $"PP {move.PP}/{move.Base.PP}";
        typeText.text = move.Base.Type.ToString();
    }

    /// <summary>
    /// Update the move selection UI with the move names
    /// </summary>
    /// <param name="moves">The list of moves </param> 
    public void SetMoveNames(List<Move> moves)
    {
        for (int i = 0; i < moveTexts.Count; ++i)
        {
            if (i < moves.Count)
                moveTexts[i].text = moves[i].Base.MoveName;
            else
                moveTexts[i].text = "-";
        }
    }
}
