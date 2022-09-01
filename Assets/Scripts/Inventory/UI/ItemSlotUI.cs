using UnityEngine;
using TMPro;

public class ItemSlotUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI countText;

    public void SetData(ItemSlot itemSlot)
    {
        nameText.text = itemSlot.Item.Name;
        countText.text = $"X {itemSlot.ItemCount}";
    }

    public TextMeshProUGUI NameText => nameText;
    public TextMeshProUGUI CountText => countText;
}
