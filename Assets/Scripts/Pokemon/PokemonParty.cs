using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// The PokemonParty class manages all pokemon parties
/// </summary>
public class PokemonParty : MonoBehaviour
{
    [SerializeField] private List<Pokemon> pokemon;

    public event Action OnUpdated;

    public List<Pokemon> Pokemon
    {
        get { return pokemon; }
        set
        {
            pokemon = value;
            OnUpdated?.Invoke();
        }
    }

    public void Awake()
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

    /// <summary>
    /// Add a pokemon to the party
    /// </summary>
    public void AddPokemon(Pokemon newPokemon)
    {
        if (pokemon.Count < 6)
        {
            pokemon.Add(newPokemon);
            OnUpdated?.Invoke();
        }
        else
        {
            // TODO: Add to PC
            Debug.Log("Added to PC");
        }
    }

    public static PokemonParty GetPlayerParty()
    {
        return FindObjectOfType<PlayerController>().GetComponent<PokemonParty>();
    }
}
