using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// The PartyScreen text manages the core party screen logic
/// </summary>
public class PartyScreen : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI messageText;

    /// <summary> The currently selected member in the party screen</summary>
    public Pokemon SelectedMember => pokemon[selectionIndex];

    private PartyMemberUI[] memberSlots;
    private List<Pokemon> pokemon;
    private PokemonParty party;

    private int selectionIndex = 0;

    /// <summary>
    /// Where the party screen was called from (ActionSelection, RunningTurn, etc)
    /// </summary>
    public BattleState? CalledFrom { get; set; }

    /// <summary>
    /// Initialize the party screen
    /// </summary>
    public void Init()
    {
        memberSlots = GetComponentsInChildren<PartyMemberUI>(true);

        party = PokemonParty.GetPlayerParty();
        SetPartyData();

        party.OnUpdated += SetPartyData;
    }

    /// <summary>
    /// Updates the party screen
    /// </summary>
    /// <param name="onSelected">The action to invoke when a member on the party screen is selected </param>
    /// <param name="onBack"> The action to invoke when the player pressed the back button </param>
    public void HandleUpdate(Action onSelected, Action onBack)
    {
        int prevSelectionIndex = selectionIndex;

        if (Input.GetKeyDown(KeyCode.RightArrow))
            ++selectionIndex;
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            --selectionIndex;
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            selectionIndex += 2;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            selectionIndex -= 2;

        selectionIndex = Mathf.Clamp(selectionIndex, 0, pokemon.Count - 1);

        if (selectionIndex != prevSelectionIndex) UpdateMemberSelection(selectionIndex);

        if (Input.GetKeyDown(KeyCode.Return))
        {
            onSelected?.Invoke();
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            onBack?.Invoke();
        }
    }

    /// <summary>
    /// Set the data for the party screen
    /// </summary>
    public void SetPartyData()
    {
        pokemon = party.Pokemon;

        for (int i = 0; i < memberSlots.Length; i++)
        {
            if (i < pokemon.Count)
            {
                memberSlots[i].gameObject.SetActive(true);
                memberSlots[i].Init(pokemon[i]);
            }
            else
                memberSlots[i].gameObject.SetActive(false);
        }

        UpdateMemberSelection(selectionIndex);

        messageText.text = "Choose a Pokemon";
    }

    /// <summary>
    /// Set the message text for the dialog box at the bottom of the party screen
    /// </summary>
    /// <param name="message">The message to display </param>
    public void SetMessageText(string message)
    {
        messageText.text = message;
    }

    /// <summary>
    /// Update the member selection UI in the party screen
    /// </summary>
    /// <param name="selectedMemberIndex">The index of the selected party member</param>
    public void UpdateMemberSelection(int selectedMemberIndex)
    {
        for (int i = 0; i < pokemon.Count; i++)
        {
            if (i == selectedMemberIndex)
                memberSlots[i].SetSelected(true);
            else
                memberSlots[i].SetSelected(false);
        }
    }

    /// <summary>
    /// Show if the TM is usable or not in the party member's party screen slot
    /// </summary>
    /// <param name="tmItem">The TM item to show </param>
    public void ShowIfTMUsable(TMItem tmItem)
    {
        for (int i = 0; i < pokemon.Count; i++)
        {
            string message = tmItem.CanBeTaught(pokemon[i]) ? "Able" : "Not able";
            memberSlots[i].SetMessageText(message);
        }
    }

    /// <summary>
    /// Clear the member slot text in the party screen
    /// </summary>
    public void ClearMemberSlotMessages()
    {
        for (int i = 0; i < pokemon.Count; i++)
        {
            memberSlots[i].SetMessageText("");
        }
    }
}
