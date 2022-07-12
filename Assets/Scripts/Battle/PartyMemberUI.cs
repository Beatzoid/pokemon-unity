using TMPro;
using UnityEngine;

public class PartyMemberUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] HPBar hpBar;
    [SerializeField] Color highlightedColor;

    Pokemon _pokemon;

    /// <summary>
    /// Set the data for the Party Member UI
    /// </summary>
    /// <param name="pokemon">The pokemon to get the data from </param> 
    public void SetData(Pokemon pokemon)
    {
        _pokemon = pokemon;

        nameText.text = pokemon.Base.Name;
        levelText.text = "Lvl " + pokemon.Level;
        hpBar.SetHP((float)pokemon.HP / pokemon.MaxHp);
    }

    /// <summary>
    /// Set whether the party member is selected (highlighted)
    /// </summary>
    /// <param name="selected">Whether to select the party member </param>
    public void SetSelected(bool selected)
    {
        if (selected)
            nameText.color = highlightedColor;
        else
            nameText.color = Color.black;
    }
}
