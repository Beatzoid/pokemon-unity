using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The InventoryUIState is used to keep track of the current state of the inventory UI
/// </summary>
public enum InventoryUIState { ItemSelection, PartySelection, Busy };

/// <summary>
/// The InventoryUI class manages the inventory UI
/// </summary>
public class InventoryUI : MonoBehaviour
{
    [SerializeField] private GameObject itemList;
    [SerializeField] private ItemSlotUI itemSlotUI;

    [SerializeField] private TextMeshProUGUI categoryText;
    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI itemDescription;

    [SerializeField] private Image upArrow;
    [SerializeField] private Image downArrow;

    [SerializeField] private PartyScreen partyScreen;

    private Action onItemUsed;

    private const int numItemsInViewport = 8;

    private int selectedItem = 0;
    private int selectedCategory = 0;

    private Inventory inventory;
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

        inventory.OnUpdated += UpdateItemList;
    }

    /// <summary>
    /// Update the InventoryUI
    /// </summary>
    /// <param name="onBack"> The action to invoke when the user pressed the back button </param>
    /// <param name="onItemUsed"> The action to invoke when the user used an item </param>
    public void HandleUpdate(Action onBack, Action onItemUsed = null)
    {
        this.onItemUsed = onItemUsed;

        if (state == InventoryUIState.ItemSelection)
        {
            if (Input.GetKeyDown(KeyCode.Return))
                OpenPartyScreen();
            else if (Input.GetKeyDown(KeyCode.X))
                onBack?.Invoke();

            int prevSelectedItem = selectedItem;
            int prevSelectedCategory = selectedCategory;

            if (Input.GetKeyDown(KeyCode.DownArrow))
                selectedItem++;
            else if (Input.GetKeyDown(KeyCode.UpArrow))
                selectedItem--;
            else if (Input.GetKeyDown(KeyCode.RightArrow))
                selectedCategory++;
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
                selectedCategory--;

            if (selectedCategory > Inventory.ItemCategories.Count - 1)
                selectedCategory = 0;
            else if (selectedCategory < 0)
                selectedCategory = Inventory.ItemCategories.Count - 1;

            selectedItem = Mathf.Clamp(selectedItem, 0, inventory.GetSlotsByCategory(selectedCategory).Count - 1);

            if (prevSelectedCategory != selectedCategory)
            {
                ResetSelection();
                categoryText.text = Inventory.ItemCategories[selectedCategory];
                UpdateItemList();
            }
            else if (prevSelectedItem != selectedItem)
                UpdateItemSelection();
        }
        else if (state == InventoryUIState.PartySelection)
        {
            void OnSelected()
            {
                Debug.Log("Selected item");
                StartCoroutine(UseItem());
            }

            void OnBackPartyScreen()
            {
                ClosePartyScreen();
            }

            partyScreen.HandleUpdate(OnSelected, OnBackPartyScreen);
        }
    }

    private IEnumerator UseItem()
    {
        state = InventoryUIState.Busy;

        ItemBase usedItem = inventory.UseItem(selectedItem, partyScreen.SelectedMember, selectedCategory);
        if (usedItem != null)
        {
            Debug.Log($"Used {usedItem.Name}");
            yield return DialogManager.Instance.ShowDialogText($"Successfully used {usedItem.Name}");
            onItemUsed?.Invoke();
        }
        else
        {
            yield return DialogManager.Instance.ShowDialogText($"It won't have any effect!");
        }

        ClosePartyScreen();
    }

    private void ResetSelection()
    {
        selectedItem = 0;

        upArrow.gameObject.SetActive(false);
        downArrow.gameObject.SetActive(false);

        itemIcon.sprite = null;
        itemDescription.text = "";
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
        List<ItemSlot> slots = inventory.GetSlotsByCategory(selectedCategory);

        selectedItem = Mathf.Clamp(selectedItem, 0, slots.Count - 1);

        for (int i = 0; i < slotUIList.Count; i++)
        {
            if (i == selectedItem)
                slotUIList[i].NameText.color = GlobalSettings.Instance.HighlightedColor;
            else
                slotUIList[i].NameText.color = Color.black;
        }

        if (slots.Count > 0)
        {
            ItemBase slotItem = slots[selectedItem].Item;

            itemIcon.sprite = slotItem.Icon;
            itemDescription.text = slotItem.Description;
        }

        HandleScrolling();
    }

    private void UpdateItemList()
    {
        foreach (Transform child in itemList.transform)
            Destroy(child.gameObject);

        slotUIList = new List<ItemSlotUI>();
        foreach (ItemSlot itemSlot in inventory.GetSlotsByCategory(selectedCategory))
        {
            ItemSlotUI slotUIObject = Instantiate(itemSlotUI, itemList.transform);

            slotUIObject.SetData(itemSlot);
            slotUIList.Add(slotUIObject);
        }

        UpdateItemSelection();
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
}
