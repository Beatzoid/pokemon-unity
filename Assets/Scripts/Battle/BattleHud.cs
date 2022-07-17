using TMPro;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// The BattleHud class manages all the HUD's in the battle scene
/// </summary>
public class BattleHud : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private HPBar hpBar;

    [Space(5)]
    [Header("Status Effect Text Colors")]
    [SerializeField] private Color poisonColor;
    [SerializeField] private Color burnColor;
    [SerializeField] private Color sleepColor;
    [SerializeField] private Color paralyzeColor;
    [SerializeField] private Color freezeColor;

    private Pokemon _pokemon;
    private Dictionary<ConditionID, Color> statusColors;

    /// <summary>
    /// Set the data for the battle HUD
    /// </summary>
    /// <param name="pokemon">The pokemon to get the data from </param>
    public void SetData(Pokemon pokemon)
    {
        _pokemon = pokemon;

        nameText.text = pokemon.Base.Name;
        levelText.text = "Lvl " + pokemon.Level;
        hpBar.SetHP((float)pokemon.HP / pokemon.MaxHp);

        statusColors = new Dictionary<ConditionID, Color>()
        {
            { ConditionID.PSN, poisonColor },
            { ConditionID.BRN, burnColor },
            { ConditionID.SLP, sleepColor },
            { ConditionID.PRZ, paralyzeColor },
            { ConditionID.FRZ, freezeColor },
        };

        SetStatusText();
        _pokemon.OnStatusChanged += SetStatusText;
    }

    /// </summary>
    /// Set the status text to the pokemon's status effect
    /// </summary>
    public void SetStatusText()
    {
        if (_pokemon.Status == null)
        {
            statusText.text = "";
        }
        else
        {
            statusText.text = _pokemon.Status.Id.ToString().ToUpper();
            statusText.color = statusColors[_pokemon.Status.Id];
        }
    }

    /// <summary>
    /// Update the HP bar smoothly
    /// </summary>
    public IEnumerator UpdateHP()
    {
        if (_pokemon.HPChanged)
        {
            yield return hpBar.SetHPSmoothly((float)_pokemon.HP / _pokemon.MaxHp);
            _pokemon.HPChanged = false;
        }
    }
}
