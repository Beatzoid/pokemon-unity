using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// The ItemCategory enum is used to keep track of the indexes of the item categories
/// </summary>
public enum ItemCategory
{
    Items, Pokeballs, TMs
}

/// <summary>
/// The ItemSlot class manages the individual item slots the Inventory
/// </summary>
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

/// <summary>
/// The Inventory class manages all Inventory-related logic
/// </summary>
public class Inventory : MonoBehaviour
{
    [SerializeField] private List<ItemSlot> slots;
    [SerializeField] private List<ItemSlot> pokeballSlots;
    [SerializeField] private List<ItemSlot> tmSlots;

    public event Action OnUpdated;

    public static List<string> ItemCategories { get; set; } = new List<string>()
    {
        "ITEMS",
        "POKEBALLS",
        "TMs & HMs"
    };

    private List<List<ItemSlot>> allSlots;

    public void Awake()
    {
        allSlots = new List<List<ItemSlot>>() { slots, pokeballSlots, tmSlots };
    }

    /// <summary>
    /// Get the List of ItemSlots for the specific category
    /// </summary>
    /// <param name="categoryIndex"> The index of the category </param>
    public List<ItemSlot> GetSlotsByCategory(int categoryIndex)
    {
        return allSlots[categoryIndex];
    }

    /// <summary>
    /// Use an item
    /// </summary>
    /// <param name="itemIndex">The index of the item to use </param>
    /// <param name="selectedPokemon">The selected pokemon to use the item on </param>
    /// <param name="selectedCategory">The index of the selected category <param>
    public ItemBase UseItem(int itemIndex, Pokemon selectedPokemon, int selectedCategory)
    {
        List<ItemSlot> currentSlots = GetSlotsByCategory(selectedCategory);

        ItemBase item = currentSlots[itemIndex].Item;
        bool itemUsed = item.Use(selectedPokemon);

        if (itemUsed)
        {
            RemoveItem(item, selectedCategory);
            return item;
        }

        return null;
    }

    /// <summary>
    /// Remove an item from the Inventory
    /// </summary>
    /// <param name="item">The item to remove </param>
    /// <param name="selectedCategory">The index of the selected category <param>
    public void RemoveItem(ItemBase item, int selectedCategory)
    {
        List<ItemSlot> currentSlots = GetSlotsByCategory(selectedCategory);

        ItemSlot itemSlot = currentSlots.First(slot => slot.Item == item);
        itemSlot.ItemCount--;
        if (itemSlot.ItemCount == 0)
            slots.Remove(itemSlot);

        OnUpdated?.Invoke();
    }

    public static Inventory GetInventory()
    {
        return FindObjectOfType<PlayerController>().GetComponent<Inventory>();
    }
}
