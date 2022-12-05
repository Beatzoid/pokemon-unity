using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class ItemSlot
{
    [SerializeField] private ItemBase item;
    [SerializeField] private int itemCount;

    public ItemBase Item => item;
    public int ItemCount
    {
        get => itemCount;
        set => itemCount = value;
    }
}

public class Inventory : MonoBehaviour
{
    [SerializeField] private List<ItemSlot> slots;

    public event Action OnUpdated;

    public List<ItemSlot> Slots => slots;

    public ItemBase UseItem(int itemIndex, Pokemon selectedPokemon)
    {
        ItemBase item = slots[itemIndex].Item;
        bool itemUsed = item.Use(selectedPokemon);

        if (itemUsed)
        {
            RemoveItem(item);
            return item;
        }

        return null;
    }

    public void RemoveItem(ItemBase item)
    {
        ItemSlot slot = slots.First(slot => slot.Item == item);
        slot.ItemCount--;
        if (slot.ItemCount == 0)
            slots.Remove(slot);

        OnUpdated?.Invoke();
    }

    public static Inventory GetInventory()
    {
        return FindObjectOfType<PlayerController>().GetComponent<Inventory>();
    }
}
