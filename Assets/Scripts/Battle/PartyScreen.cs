using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// The PartyScreen text manages the core party screen logic
/// </summary>
public class PartyScreen : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI messageText;

    private PartyMemberUI[] memberSlots;
    private List<Pokemon> pokemon;

    /// <summary>
    /// Initialize the party screen
    /// </summary>
    public void Init()
    {
        memberSlots = GetComponentsInChildren<PartyMemberUI>(true);
    }

    /// <summary>
    /// Set the data for the party screen
    /// </summary>
    /// <param name="pokemon">The pokemon to display </param>
    public void SetPartyData(List<Pokemon> pokemon)
    {
        this.pokemon = pokemon;

        for (int i = 0; i < memberSlots.Length; i++)
        {
            if (i < pokemon.Count)
            {
                memberSlots[i].gameObject.SetActive(true);
                memberSlots[i].SetData(pokemon[i]);
            }
            else
                memberSlots[i].gameObject.SetActive(false);
        }

        messageText.text = "Choose a Pokemon";
    }

    /// <summary>
    /// Set the message text for the party screen
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
}
