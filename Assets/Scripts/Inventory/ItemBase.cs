using UnityEngine;

public class ItemBase : ScriptableObject
{
    [SerializeField] private string itemName;
    [SerializeField] private string description;
    [SerializeField] private Sprite icon;

    public string Name => name;
    public string Description => description;
    public Sprite Icon => icon;
}
