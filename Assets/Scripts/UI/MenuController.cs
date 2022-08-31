using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    [SerializeField] private GameObject menu;

    public event Action<int> onMenuSelected;
    public event Action onBack;

    private List<TextMeshProUGUI> menuItems;
    private int selectedItem;

    public void Awake()
    {
        selectedItem = 0;
        menuItems = menu.GetComponentsInChildren<TextMeshProUGUI>().ToList();
    }

    public void Start()
    {
        UpdateItemSelection();
    }

    public void HandleUpdate()
    {
        int prevSelectedItem = selectedItem;
        if (Input.GetKeyDown(KeyCode.DownArrow))
            ++selectedItem;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            selectedItem--;

        selectedItem = Mathf.Clamp(selectedItem, 0, menuItems.Count - 1);

        if (prevSelectedItem != selectedItem)
            UpdateItemSelection();

        if (Input.GetKeyDown(KeyCode.Return))
        {
            onMenuSelected?.Invoke(selectedItem);
            CloseMenu();
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            onBack?.Invoke();
            CloseMenu();
        }
    }

    public void OpenMenu()
    {
        menu.SetActive(true);
    }

    public void CloseMenu()
    {
        menu.SetActive(false);
    }


    private void UpdateItemSelection()
    {
        for (int i = 0; i < menuItems.Count; i++)
        {
            if (i == selectedItem)
                menuItems[i].color = GlobalSettings.I.HighlightedColor;
            else
                menuItems[i].color = Color.black;
        }
    }
}
