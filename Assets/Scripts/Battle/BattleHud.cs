using TMPro;
using System.Collections;
using UnityEngine;

/// <summary>
/// The BattleHud class manages all the HUD's in the battle scene
/// </summary>
public class BattleHud : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private HPBar hpBar;
    private Pokemon _pokemon;

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
    }

    /// <summary>
    /// Update the HP bar smoothly
    /// </summary>
    public IEnumerator SmoothlyUpdateHP()
    {
        yield return hpBar.SetHPSmoothly((float)_pokemon.HP / _pokemon.MaxHp);
    }
}
