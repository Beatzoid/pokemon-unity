using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// The PokemonParty class manages all pokemon parties
/// </summary>
public class PokemonParty : MonoBehaviour
{
    [SerializeField] private List<Pokemon> pokemon;

    public List<Pokemon> Pokemon
    {
        get { return pokemon; }
    }

    public void Start()
    {
        foreach (Pokemon pokemon in pokemon)
        {
            pokemon.Init();
        }
    }

    /// <summary>
    /// Returns the first pokemon in the party that has not fainted
    /// </summary>
    public Pokemon GetHealthyPokemon()
    {
        return pokemon.FirstOrDefault(pokemon => pokemon.HP > 0);
    }
}
