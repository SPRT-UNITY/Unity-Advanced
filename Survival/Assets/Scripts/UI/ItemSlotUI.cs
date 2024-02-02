using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ItemSlot
{
    public ItemData itemData;
    public int quantity;
}


public class ItemSlotUI : MonoBehaviour
{
    public Button button;
    public Image icon;
    public TextMeshProUGUI quantityText;

    private ItemSlot curSlot;
    private Outline outline;

    public int index;
    public bool equipped;

    private void Awake()
    {
        outline = GetComponent<Outline>();
    }

    private void OnEnable()
    {
        outline.enabled = equipped;
    }

    public void Set(ItemSlot itemSlot) 
    {
        curSlot = itemSlot;
        icon.gameObject.SetActive(true);
        icon.sprite = itemSlot.itemData.icon;
        quantityText.text = itemSlot.quantity > 1 ? itemSlot.quantity.ToString() : string.Empty;

        if(outline != null) 
        {
            outline.enabled = equipped;
        }
    }

    public void Clear() 
    {
        curSlot = null;
        icon.gameObject.SetActive(false);
        quantityText.text = string.Empty;
    }

    public void OnButtonClick() 
    {
        Inventory.instance.SelectItem(index);
    }
}
