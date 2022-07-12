using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PokemonParty : MonoBehaviour
{
    [SerializeField] List<Pokemon> pokemon;

    public void Start()
    {
        foreach (Pokemon pokemon in pokemon)
        {
            pokemon.Init();
        }
    }

    /// <summary>
    /// Returns the first pokemon in the party
    /// that has not fainted
    /// </summary>
    public Pokemon GetHealthyPokemon()
    {
        return pokemon.FirstOrDefault(pokemon => pokemon.HP > 0);
    }
}
