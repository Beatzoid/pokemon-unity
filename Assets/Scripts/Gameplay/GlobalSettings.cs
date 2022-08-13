using UnityEngine;

public class GlobalSettings : MonoBehaviour
{
    [SerializeField] private Color highlightedColor;

    public static GlobalSettings I { get; private set; }

    public void Awake()
    {
        I = this;
    }

    public Color HighlightedColor => highlightedColor;
}
