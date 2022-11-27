using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum InventoryUIState { ItemSelection, PartySelection, Busy };

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private GameObject itemList;
    [SerializeField] private ItemSlotUI itemSlotUI;

    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI itemDescription;

    [SerializeField] private Image upArrow;
    [SerializeField] private Image downArrow;

    [SerializeField] private PartyScreen partyScreen;

    private const int numItemsInViewport = 8;

    private Inventory inventory;

    private int selectedItem;
    private List<ItemSlotUI> slotUIList;

    private RectTransform itemListRect;

    private InventoryUIState state;

    public void Awake()
    {
        selectedItem = 0;
        inventory = Inventory.GetInventory();
        itemListRect = itemList.GetComponent<RectTransform>();
    }

    public void Start()
    {
        UpdateItemList();
    }

    public void HandleUpdate(Action onBack)
    {
        if (state == InventoryUIState.ItemSelection)
        {
            if (Input.GetKeyDown(KeyCode.Return))
                OpenPartyScreen();
            else if (Input.GetKeyDown(KeyCode.X))
                onBack?.Invoke();

            int prevSelectedItem = selectedItem;

            if (Input.GetKeyDown(KeyCode.DownArrow))
                ++selectedItem;
            else if (Input.GetKeyDown(KeyCode.UpArrow))
                selectedItem--;

            selectedItem = Mathf.Clamp(selectedItem, 0, inventory.Slots.Count - 1);

            if (prevSelectedItem != selectedItem)
                UpdateItemSelection();
        }
        else if (state == InventoryUIState.PartySelection)
        {
            static void OnSelected()
            {
                // Use the item on the pokemon
            }

            void OnBackPartyScreen()
            {
                ClosePartyScreen();
            }

            partyScreen.HandleUpdate(OnSelected, OnBackPartyScreen);
        }
    }

    private void OpenPartyScreen()
    {
        state = InventoryUIState.PartySelection;

        partyScreen.gameObject.SetActive(true);
    }

    private void ClosePartyScreen()
    {
        state = InventoryUIState.ItemSelection;

        partyScreen.gameObject.SetActive(false);
    }

    private void UpdateItemSelection()
    {
        for (int i = 0; i < slotUIList.Count; i++)
        {
            if (i == selectedItem)
                slotUIList[i].NameText.color = GlobalSettings.I.HighlightedColor;
            else
                slotUIList[i].NameText.color = Color.black;
        }

        ItemBase slotItem = inventory.Slots[selectedItem].Item;
        itemIcon.sprite = slotItem.Icon;
        itemDescription.text = slotItem.Description;

        HandleScrolling();
    }

    private void HandleScrolling()
    {
        if (slotUIList.Count <= numItemsInViewport) return;

        float scrollPos = Mathf.Clamp(selectedItem - (numItemsInViewport / 2), 0, selectedItem) * slotUIList[0].Height;

        itemListRect.localPosition = new Vector2(itemListRect.localPosition.x, scrollPos);

        bool showUpArrow = selectedItem > numItemsInViewport / 2;
        bool showDownArrow = selectedItem + (numItemsInViewport / 2) < slotUIList.Count;

        upArrow.gameObject.SetActive(showUpArrow);
        downArrow.gameObject.SetActive(showDownArrow);
    }

    private void UpdateItemList()
    {
        foreach (Transform child in itemList.transform)
            Destroy(child.gameObject);

        slotUIList = new List<ItemSlotUI>();
        foreach (ItemSlot itemSlot in inventory.Slots)
        {
            ItemSlotUI slotUIObject = Instantiate(itemSlotUI, itemList.transform);

            slotUIObject.SetData(itemSlot);
            slotUIList.Add(slotUIObject);
        }

        UpdateItemSelection();
    }
}
