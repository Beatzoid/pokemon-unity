using TMPro;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

/// <summary>
/// The BattleHud class manages all the HUD's in the battle scene
/// </summary>
public class BattleHud : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private HPBar hpBar;
    [SerializeField] private GameObject expBar;

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
        hpBar.SetHP((float)pokemon.HP / pokemon.MaxHp);
        SetLevel();
        SetExp();

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
    /// Set the exp on the exp bar
    /// </summary>
    public void SetExp()
    {
        if (expBar == null) return;

        float normalizedExp = GetNormalizedExp();
        expBar.transform.localScale = new Vector3(normalizedExp, 1, 1);
    }

    public void SetLevel()
    {
        levelText.text = "Lvl " + _pokemon.Level;
    }

    /// <summary>
    /// Set the exp on the exp bar smoothly
    /// </summary>
    public IEnumerator SetExpSmooth(bool reset = false)
    {
        if (expBar == null) yield break;

        if (reset)
            expBar.transform.localScale = new Vector3(0, 1, 1);

        float normalizedExp = GetNormalizedExp();
        yield return expBar.transform.DOScaleX(normalizedExp, 1.5f).WaitForCompletion();
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
    private float GetNormalizedExp()
    {
        int currentLevelExp = _pokemon.Base.GetExpForLevel(_pokemon.Level);
        int nextLevelExp = _pokemon.Base.GetExpForLevel(_pokemon.Level + 1);

        float normalizedExp = (float)(_pokemon.Exp - currentLevelExp) / (nextLevelExp - currentLevelExp);
        return Mathf.Clamp01(normalizedExp);
    }
}
