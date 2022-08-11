using System.Collections.Generic;
using UnityEngine;

public class PokemonDB
{
    private static Dictionary<string, PokemonBase> pokemon;

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
