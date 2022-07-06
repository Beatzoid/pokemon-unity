using UnityEngine;
using UnityEngine.UI;

public class BattleUnit : MonoBehaviour
{
    [SerializeField] readonly PokemonBase _base;
    [SerializeField] readonly int level;
    [SerializeField] readonly bool isPlayerUnit;
    public Pokemon Pokemon { get; set; }

    /// <summary>
    /// Setup the battle unit
    /// </summary>
    public void Setup()
    {
        Pokemon = new Pokemon(_base, level);

        if (isPlayerUnit)
            GetComponent<Image>().sprite = Pokemon.Base.BackSprite;
        else
            GetComponent<Image>().sprite = Pokemon.Base.FrontSprite;
    }
}
