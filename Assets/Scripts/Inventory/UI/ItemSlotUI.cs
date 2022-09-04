using UnityEngine;
using TMPro;

public class ItemSlotUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI countText;

    private RectTransform rectTransform;

    public void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void SetData(ItemSlot itemSlot)
    {
        nameText.text = itemSlot.Item.Name;
        countText.text = $"X {itemSlot.ItemCount}";
    }

    public float Height => rectTransform.rect.height;
    public TextMeshProUGUI NameText => nameText;
    public TextMeshProUGUI CountText => countText;
}
