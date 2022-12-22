using UnityEngine;
using TMPro;

/// <summary>
/// The ItemSlotUI class manages all the ItemSlot UI
/// </summary>
public class ItemSlotUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI countText;

    public float Height => rectTransform.rect.height;
    public TextMeshProUGUI NameText => nameText;
    public TextMeshProUGUI CountText => countText;

    private RectTransform rectTransform;

    public void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    /// <summary>
    /// Set the data for the UI
    /// </summary>
    /// <param name="itemSlot"> The ItemSlot to get the data from </param>
    public void SetData(ItemSlot itemSlot)
    {
        nameText.text = itemSlot.Item.Name;
        countText.text = $"X {itemSlot.ItemCount}";
    }
}
