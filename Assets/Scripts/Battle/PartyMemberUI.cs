using TMPro;
using UnityEngine;

/// <summary>
/// The PartyMemberUI class manages the individual party members UI in the party screen
/// </summary>
public class PartyMemberUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private HPBar hpBar;

    private Pokemon _pokemon;

    /// <summary>
    /// Initializes the party member UI with the specified pokemon's data
    /// </summary>
    /// <param name="pokemon">The pokemon to get the data from </param>
    public void Init(Pokemon pokemon)
    {
        _pokemon = pokemon;
        UpdateData();
        SetMessageText("");

        _pokemon.OnHPChanged += UpdateData;
    }

    /// <summary>
    /// Set whether the party member is selected (highlighted)
    /// </summary>
    /// <param name="selected">Whether to select the party member </param>
    public void SetSelected(bool selected)
    {
        if (selected)
            nameText.color = GlobalSettings.Instance.HighlightedColor;
        else
            nameText.color = Color.black;
    }

    /// <summary>
    /// Set the text of the message to display below the party members UI in the party screen
    /// </summary>
    /// <param name="message">The message to display </param>
    public void SetMessageText(string message)
    {
        messageText.text = message;
    }

    private void UpdateData()
    {
        nameText.text = _pokemon.Base.Name;
        levelText.text = "Lvl " + _pokemon.Level;
        hpBar.SetHP((float)_pokemon.HP / _pokemon.MaxHp);
    }
}
