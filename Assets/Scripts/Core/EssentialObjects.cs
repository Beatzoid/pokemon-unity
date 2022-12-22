using UnityEngine;

/// <summary>
/// The EssentialObjects class manages the essential objects gameobject in the scene.
/// This gameobject holds all the gameobject's that need to have both themselves and their
/// state persisted across scene loads (player, NPCs, pokemon, etc).
/// </summary>
public class EssentialObjects : MonoBehaviour
{
    public void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
