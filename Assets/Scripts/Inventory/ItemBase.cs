using UnityEngine;

/// <summary>
/// The ItemBase class manage the basic data for all items
/// </summary>
public class ItemBase : ScriptableObject
{
    [SerializeField] private string itemName;
    [SerializeField] private string description;
    [SerializeField] private Sprite icon;

    public virtual string Name => name;
    public virtual string Description => description;
    public Sprite Icon => icon;

    public virtual bool CanUseInBattle => true;
    public virtual bool CanUseOutsideBattle => true;

    public virtual bool IsReusable => false;

    public virtual bool Use(Pokemon pokemon)
    {
        return false;
    }
}
