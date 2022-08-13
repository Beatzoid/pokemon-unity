using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// The BattleDialogBox manages the dialog box in the battle scene
/// </summary>
public class BattleDialogBox : MonoBehaviour
{
    [Header("Settings")]

    [SerializeField] private int lettersPerSecond;

    [Space(5)]
    [Header("Objects")]

    [SerializeField] private GameObject actionSelector;
    [SerializeField] private GameObject moveSelector;
    [SerializeField] private GameObject moveDetails;
    [SerializeField] private GameObject choiceBox;

    [Space(5)]
    [Header("Text")]

    [SerializeField] private List<TextMeshProUGUI> actionText;
    [SerializeField] private List<TextMeshProUGUI> moveText;

    [SerializeField] private TextMeshProUGUI dialogText;

    [SerializeField] private TextMeshProUGUI ppText;
    [SerializeField] private TextMeshProUGUI typeText;

    [SerializeField] private TextMeshProUGUI yesText;
    [SerializeField] private TextMeshProUGUI noText;

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

        yield return new WaitForSeconds(1f);
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
    public void SetActionSelectorActive(bool enabled)
    {
        actionSelector.SetActive(enabled);
    }

    /// <summary>
    /// Show/hide the move selector and details
    /// </summary>
    /// <param name="enabled">Whether to show or hide the move selector amd details </param>
    public void SetMoveSelectorActive(bool enabled)
    {
        moveSelector.SetActive(enabled);
        moveDetails.SetActive(enabled);
    }

    /// <summary>
    /// Show/hide the choice box
    /// </summary>
    /// <param name="enabled">Whether to show or hide the choice box </param>
    public void SetChoiceBoxActive(bool enabled)
    {
        choiceBox.SetActive(enabled);
    }

    /// <summary>
    /// Update the move selection UI with the move names
    /// </summary>
    /// <param name="moves">The list of moves </param>
    public void SetMoveNames(List<Move> moves)
    {
        for (int i = 0; i < moveText.Count; ++i)
        {
            if (i < moves.Count)
                moveText[i].text = moves[i].Base.MoveName;
            else
                moveText[i].text = "-";
        }
    }

    /// <summary>
    /// Update the action selection UI with the selected action
    /// </summary>
    /// <param name="selectedAction">The selected action </param>
    public void UpdateActionSelection(int selectedAction)
    {
        for (int i = 0; i < actionText.Count; ++i)
        {
            if (i == selectedAction)
                actionText[i].color = GlobalSettings.I.HighlightedColor;
            else
                actionText[i].color = Color.black;
        }
    }

    /// <summary>
    /// Update the move selection UI with the selected move
    /// </summary>
    /// <param name="selectedMove">The index of the selected move </param>
    /// <param name="move">The selected move </param>
    public void UpdateMoveSelection(int selectedMove, Move move)
    {
        for (int i = 0; i < moveText.Count; ++i)
        {
            if (i == selectedMove)
                moveText[i].color = GlobalSettings.I.HighlightedColor;
            else
                moveText[i].color = Color.black;
        }

        ppText.text = $"PP {move.PP}/{move.Base.PP}";
        typeText.text = move.Base.Type.ToString();

        if (move.PP == 0)
            ppText.color = Color.red;
        else if (move.PP <= move.Base.PP / 2)
            ppText.color = new Color(1f, 0.647f, 0f);
        else
            ppText.color = Color.black;
    }

    /// <summary>
    /// Update the choice box
    /// </summary>
    /// <param name="selectedChoice">The selected choice </param>
    public void UpdateChoiceBox(bool yesSelected)
    {
        if (yesSelected)
        {
            yesText.color = GlobalSettings.I.HighlightedColor;
            noText.color = Color.black;
        }
        else
        {
            yesText.color = Color.black;
            noText.color = GlobalSettings.I.HighlightedColor;
        }
    }
}
