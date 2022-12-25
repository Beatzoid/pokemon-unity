using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The InventoryUIState is used to keep track of the current state of the inventory UI
/// </summary>
public enum InventoryUIState { ItemSelection, PartySelection, MoveToForget, Busy };

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

    [SerializeField] private MoveSelectionUI moveSelectionUI;

    private Action<ItemBase> onItemUsed;

    private const int numItemsInViewport = 8;

    private int selectedItem = 0;
    private int selectedCategory = 0;

    private Inventory inventory;
    private List<ItemSlotUI> slotUIList;
    private RectTransform itemListRect;

    private InventoryUIState state;

    private MoveBase moveToLearn;

    private bool usedTM = false;

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
    /// Update the Inventory UI
    /// </summary>
    /// <param name="onBack"> The action to invoke when the user pressed the back button </param>
    /// <param name="onItemUsed"> The action to invoke when the user used an item </param>
    public void HandleUpdate(Action onBack, Action<ItemBase> onItemUsed = null)
    {
        this.onItemUsed = onItemUsed;

        if (state == InventoryUIState.ItemSelection)
        {
            if (Input.GetKeyDown(KeyCode.Return))
                StartCoroutine(ItemSelected());
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
        else if (state == InventoryUIState.MoveToForget)
        {
            void OnMoveSelected(int moveIndex)
            {
                StartCoroutine(OnMoveToForgetSelected(moveIndex));
            }

            moveSelectionUI.HandleMoveSelection(OnMoveSelected);
        }
    }

    private IEnumerator OnMoveToForgetSelected(int moveIndex)
    {
        Pokemon pokemon = partyScreen.SelectedMember;

        DialogManager.Instance.CloseDialog();
        moveSelectionUI.gameObject.SetActive(false);

        // The player selected the same move that they are trying to learn
        if (moveIndex == PokemonBase.MaxNumOfMoves)
        {
            yield return DialogManager.Instance.ShowDialogText($"{pokemon.Base.Name} did not learn {moveToLearn.MoveName}");
        }
        else
        {
            MoveBase selectedMove = pokemon.Moves[moveIndex].Base;
            yield return DialogManager.Instance.ShowDialogText(
                $"{pokemon.Base.Name} forgot {selectedMove.MoveName} and learned {moveToLearn.MoveName}"
            );

            Debug.Log(moveToLearn.PP);
            pokemon.Moves[moveIndex] = new Move(moveToLearn);
        }

        moveToLearn = null;
        state = InventoryUIState.ItemSelection;
    }

    private IEnumerator UseItem()
    {
        state = InventoryUIState.Busy;

        yield return HandleTMItems();

        // With TM items, even if the pokemon didn't learn the move this code still gets executed
        // so if this check wasn't here the item would keep getting used and the item count
        // would go into the negatives
        if (!usedTM)
        {
            ClosePartyScreen();
            yield break;
        }


        ItemBase usedItem = inventory.UseItem(selectedItem, partyScreen.SelectedMember, selectedCategory);

        if (usedItem != null)
        {
            if (usedItem is RecoveryItem)
                yield return DialogManager.Instance.ShowDialogText($"{GameController.Instance.Player.Name} used {usedItem.Name}!");

            onItemUsed?.Invoke(usedItem);
        }
        else
        {
            if (usedItem is RecoveryItem)
                yield return DialogManager.Instance.ShowDialogText($"It won't have any effect!");
        }

        ClosePartyScreen();
    }

    private IEnumerator HandleTMItems()
    {
        usedTM = false;

        TMItem tmItem = inventory.GetItem(selectedItem, selectedCategory) as TMItem;
        // If we try to cast to TMItem and it is not already a TMItem (e.x RecoveryItem, PokeballItem, etc)
        /// then tmItem will equal null
        if (tmItem == null)
            yield break;

        Pokemon pokemon = partyScreen.SelectedMember;

        if (pokemon.HasMove(tmItem.Move))
        {
            yield return DialogManager.Instance.ShowDialogText($"{pokemon.Base.Name} already knows {tmItem.Move.MoveName}!");
            yield break;
        }

        if (!tmItem.CanBeTaught(pokemon))
        {
            yield return DialogManager.Instance.ShowDialogText($"{pokemon.Base.Name} cannot learn {tmItem.Move.MoveName}!");
            yield break;
        }

        if (pokemon.Moves.Count < PokemonBase.MaxNumOfMoves)
        {
            pokemon.LearnMove(tmItem.Move);
            usedTM = true;
            yield return DialogManager.Instance.ShowDialogText($"{pokemon.Base.Name} learned {tmItem.Move.MoveName}");
        }
        else
        {
            yield return DialogManager.Instance.ShowDialogText($"{pokemon.Base.Name} is trying to learn {tmItem.Move.MoveName}");
            yield return DialogManager.Instance.ShowDialogText($"But it cannot learn more than {PokemonBase.MaxNumOfMoves} moves!");
            yield return ChooseMoveToForget(pokemon, tmItem.Move);
            yield return new WaitUntil(() => state != InventoryUIState.MoveToForget);
            usedTM = true;
        }
    }

    private IEnumerator ChooseMoveToForget(Pokemon pokemon, MoveBase newMove)
    {
        state = InventoryUIState.Busy;
        yield return DialogManager.Instance.ShowDialogText("Choose a move you want to forget", autoClose: false);

        moveSelectionUI.gameObject.SetActive(true);
        moveSelectionUI.SetMoveData(pokemon.Moves.Select(x => x.Base).ToList(), newMove);

        moveToLearn = newMove;

        state = InventoryUIState.MoveToForget;
    }

    private IEnumerator ItemSelected()
    {
        state = InventoryUIState.Busy;

        ItemBase item = inventory.GetItem(selectedItem, selectedCategory);

        if (GameController.Instance.State == GameState.Battle)
        {
            if (!item.CanUseInBattle)
            {
                yield return DialogManager.Instance.ShowDialogText("This item cannot be used in battle");
                state = InventoryUIState.ItemSelection;
                yield break;
            }
        }
        else
        {
            if (!item.CanUseOutsideBattle)
            {
                yield return DialogManager.Instance.ShowDialogText("This item cannot be used outside battle");
                state = InventoryUIState.ItemSelection;
                yield break;
            }
        }

        if (selectedCategory == (int)ItemCategory.Pokeballs)
        {
            StartCoroutine(UseItem());
        }
        else
        {
            OpenPartyScreen();

            if (item is TMItem)
                partyScreen.ShowIfTMUsable(item as TMItem);
        }
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

        partyScreen.ClearMemberSlotMessages();
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
