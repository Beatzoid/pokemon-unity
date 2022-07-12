using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapArea : MonoBehaviour
{
    [SerializeField] List<Pokemon> wildPokemon;

    /// <summary>
    /// Get a wild pokemon from the map area
    /// </summary>
    public Pokemon GetRandomWildPokemon()
    {
        Pokemon randomPokemon = wildPokemon[Random.Range(0, wildPokemon.Count)];
        randomPokemon.Init();
        return randomPokemon;
    }
}
