using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The MapArea class manages all map-related data
/// </summary>
public class MapArea : MonoBehaviour
{
    [SerializeField] private List<Pokemon> wildPokemon;

    /// <summary>
    /// Get a wild pokemon from the current map area
    /// </summary>
    public Pokemon GetRandomWildPokemon()
    {
        Pokemon randomPokemon = wildPokemon[Random.Range(0, wildPokemon.Count)];
        randomPokemon.Init();
        return randomPokemon;
    }
}
