using TMPro;
using UnityEngine;

public class BattleHud : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] HPBar hPBar;

    public void SetData(Pokemon pokemon)
    {
        nameText.text = pokemon.Base.PokemonName;
        levelText.text = "Lvl " + pokemon.Level;
        hPBar.SetHP((float)pokemon.HP / pokemon.MaxHp);
    }
}
