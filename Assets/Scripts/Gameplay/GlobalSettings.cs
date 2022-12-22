using UnityEngine;

/// <summary>
/// The GlobalSettings class manages all global settings for the game
/// </summary>
public class GlobalSettings : MonoBehaviour
{
    [SerializeField] private Color highlightedColor;

    public static GlobalSettings Instance { get; private set; }

    public void Awake()
    {
        Instance = this;
    }

    public Color HighlightedColor => highlightedColor;
}
