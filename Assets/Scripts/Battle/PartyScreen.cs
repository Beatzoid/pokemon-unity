using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PartyScreen : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI messageText;

    PartyMemberUI[] memberSlots;

    /// <summary>
    /// Initialize the party screen
    /// </summary>
    public void Init()
    {
        memberSlots = GetComponentsInChildren<PartyMemberUI>();
    }

    /// <summary>
    /// Set the data for the party screen
    /// </summary>
    /// <param name="pokemon">The pokemon to display </param>
    public void SetPartyData(List<Pokemon> pokemon)
    {
        for (int i = 0; i < memberSlots.Length; i++)
        {
            if (i < pokemon.Count)
                memberSlots[i].SetData(pokemon[i]);
            else
                memberSlots[i].gameObject.SetActive(false);
        }

        messageText.text = "Choose a Pokemon";
    }
}
