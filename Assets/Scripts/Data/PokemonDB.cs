using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The PokemonDB class manages all the pokemon
/// </summary>
public class PokemonDB
{
    private static Dictionary<string, PokemonBase> pokemon;

    /// <summary>
    /// Initialize the Pokemon DB
    /// </summary>
    public static void Init()
    {
        pokemon = new();

        PokemonBase[] pokemonArray = Resources.LoadAll<PokemonBase>("");

        foreach (PokemonBase pokemonBase in pokemonArray)
        {
            if (pokemon.ContainsKey(pokemonBase.Name))
            {
                Debug.LogError($"There are two pokemon with the name {pokemonBase.Name}");
                continue;
            }

            pokemon[pokemonBase.Name] = pokemonBase;
        }
    }

    /// <summary>
    /// Get a pokemon by it's name
    /// </summary>
    /// <param name="name">The name of the pokemon to get </param>
    public static PokemonBase GetPokemonByName(string name)
    {
        if (!pokemon.ContainsKey(name))
        {
            Debug.LogError($"Pokemon with name {name} not found in the database");
            return null;
        }

        return pokemon[name];
    }
}
