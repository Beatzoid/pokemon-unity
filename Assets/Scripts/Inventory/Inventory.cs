using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ItemSlot
{
    [SerializeField] private ItemBase item;
    [SerializeField] private int itemCount;

    public ItemBase Item => item;
    public int ItemCount => itemCount;
}

public class Inventory : MonoBehaviour
{
    [SerializeField] private List<ItemSlot> slots;

    public static Inventory GetInventory()
    {
        return FindObjectOfType<PlayerController>().GetComponent<Inventory>();
    }

    public List<ItemSlot> Slots => slots;
}
