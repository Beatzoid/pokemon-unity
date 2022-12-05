using TMPro;
using UnityEngine;

/// <summary>
/// The PartyMemberUI class manages all the UI in the party member screen
/// </summary>
public class PartyMemberUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private HPBar hpBar;

    private Pokemon _pokemon;

    /// <summary>
    /// Set the data for the Party Member UI
    /// </summary>
    /// <param name="pokemon">The pokemon to get the data from </param>
    public void Init(Pokemon pokemon)
    {
        _pokemon = pokemon;
        UpdateData();

        _pokemon.OnHPChanged += UpdateData;
    }

    /// <summary>
    /// Set whether the party member is selected (highlighted)
    /// </summary>
    /// <param name="selected">Whether to select the party member </param>
    public void SetSelected(bool selected)
    {
        if (selected)
            nameText.color = GlobalSettings.I.HighlightedColor;
        else
            nameText.color = Color.black;
    }

    private void UpdateData()
    {
        nameText.text = _pokemon.Base.Name;
        levelText.text = "Lvl " + _pokemon.Level;
        hpBar.SetHP((float)_pokemon.HP / _pokemon.MaxHp);
    }
}
