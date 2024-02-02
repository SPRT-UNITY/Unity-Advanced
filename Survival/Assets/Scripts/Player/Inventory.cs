using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using static UnityEditor.Progress;

public class Inventory : MonoBehaviour
{
    public ItemSlotUI[] uiSlot;
    public ItemSlot[] slots;

    public GameObject inventoryWindow;
    public Transform dropPosition;

    [Header("Selected Item")]
    private ItemSlot selectedItem;
    private int selectedItemIndex;
    public TextMeshProUGUI selectedItemName;
    public TextMeshProUGUI selectedItemDescription;
    public TextMeshProUGUI selectedItemStatNames;
    public TextMeshProUGUI selectedItemStatValues;

    public GameObject useButton;
    public GameObject equipButton;
    public GameObject unequipButton;
    public GameObject dropButton;

    private int curEquipIndex;

    private PlayerController playerController;
    private PlayerConditions playerConditions;

    [Header("Events")]
    public UnityEvent onOpenInventory;
    public UnityEvent onCloseInventory;

    public static Inventory instance;

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        playerController = GetComponent<PlayerController>();
        playerConditions = GetComponent<PlayerConditions>();
    }

    private void Start()
    {
        inventoryWindow.SetActive(false);
        slots = new ItemSlot[uiSlot.Length];

        for(int i = 0; i < slots.Length; i++) 
        {
            slots[i] = new ItemSlot();
            uiSlot[i].index = i;
            uiSlot[i].Clear();
        }

        ClearSelectedItemWindow();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnInventoryButton(InputAction.CallbackContext callbackContext) 
    {
        if(callbackContext.phase == InputActionPhase.Started) 
        {
            Toggle();
        }
    }

    public void Toggle() 
    {
        if (inventoryWindow.activeInHierarchy) 
        {
            inventoryWindow.SetActive(false);
            onCloseInventory.Invoke();
            playerController.ToggleCursor(false);
        }
        else 
        {
            inventoryWindow.SetActive(true);
            onOpenInventory.Invoke();
            playerController.ToggleCursor(true);
        }
    }

    public bool IsOpen() 
    {
        return inventoryWindow.activeInHierarchy;
    }

    public void AddItem(ItemData item) 
    {
        if (item.canStack) 
        {
            ItemSlot slotToStackTo = GetItemStack(item);
            if(slotToStackTo != null) 
            {
                slotToStackTo.quantity++;
                UpdateUI();
                return;
            }
        }

        ItemSlot emptySlot = GetEmptySlot();

        if(emptySlot != null) 
        {
            emptySlot.itemData = item;
            emptySlot.quantity = 1;
            UpdateUI();
            return;
        }

        ThrowItem(item);
    }

    public void ThrowItem(ItemData item) 
    {
        Instantiate(item.dropPrefab, dropPosition.position, Quaternion.Euler(Vector3.one * UnityEngine.Random.value * 360f));
    }

    void UpdateUI() 
    {
        for(int i = 0; i < slots.Length; i++) 
        {
            if (slots[i].itemData != null)
            {
                uiSlot[i].Set(slots[i]);
            }
            else
                uiSlot[i].Clear();
        }
    }

    ItemSlot GetItemStack(ItemData item) 
    {
        for(int i = 0; i < slots.Length; i++) 
        {
            if (slots[i].itemData == item && slots[i].quantity < item.maxStackAmount) 
            {
                return slots[i];
            }
        }
        return null;
    }

    ItemSlot GetEmptySlot() 
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].itemData == null)
            {
                return slots[i];
            }
        }
        return null;
    }

    public void SelectItem(int index) 
    {
        if (slots[index].itemData == null)
            return;

        selectedItem = slots[index];
        selectedItemIndex = index;

        selectedItemName.text = selectedItem.itemData.displayName;
        selectedItemDescription.text = selectedItem.itemData.description;

        for(int i=0; i < selectedItem.itemData.consumables.Length; i++) 
        {
            selectedItemStatNames.text += selectedItem.itemData.consumables[i].type.ToString() + Environment.NewLine;
            selectedItemStatValues.text += selectedItem.itemData.consumables[i].value.ToString() + Environment.NewLine;
        }

        useButton.SetActive(selectedItem.itemData.type == ItemType.Consumable);
        equipButton.SetActive(selectedItem.itemData.type == ItemType.Equipable && !uiSlot[index].equipped);
        unequipButton.SetActive(selectedItem.itemData.type == ItemType.Equipable && uiSlot[index].equipped);
        dropButton.SetActive(true);
    }

    private void ClearSelectedItemWindow()
    {

        selectedItem = null;

        selectedItemName.text = string.Empty;
        selectedItemDescription.text = string.Empty;

        selectedItemStatNames.text = string.Empty;
        selectedItemStatValues.text = string.Empty;

        useButton.SetActive(false);
        equipButton.SetActive(false);
        unequipButton.SetActive(false);
        dropButton.SetActive(true);

        UpdateUI();
    }

    public void OnUseButton() 
    {
        if(selectedItem.itemData.type == ItemType.Consumable) 
        {
            for(int i = 0; i < selectedItem.itemData.consumables.Length; i++) 
            {
                switch (selectedItem.itemData.consumables[i].type) 
                {
                    case ConsumableType.Health:
                        playerConditions.Heal(selectedItem.itemData.consumables[i].value); 
                        break;
                    case ConsumableType.Hunger:
                        playerConditions.Eat(selectedItem.itemData.consumables[i].value);
                        break;
                }
            }
        }
        RemoveSelectedItem();
    }

    public void OnEquipButton()
    {
        if (uiSlot[curEquipIndex].equipped) 
        {
            UnEquip(curEquipIndex);
        }

        uiSlot[selectedItemIndex].equipped = true;
        curEquipIndex = selectedItemIndex;
        EquipManager.instance.EquipNew(selectedItem.itemData);
        UpdateUI();

        SelectItem(selectedItemIndex);
    }

    public void OnUnequipButton() 
    {
        UnEquip(selectedItemIndex);
    }

    void UnEquip(int index) 
    {
        uiSlot[index].equipped = false;
        EquipManager.instance.UnEquip();
        UpdateUI();

        if(selectedItemIndex == index)
            SelectItem(index);
    }

    public void OnDropButton() 
    {
        if(selectedItem != null) 
        {
            ThrowItem(selectedItem.itemData);
            RemoveSelectedItem();
        }
    }

    private void RemoveSelectedItem()
    {
        selectedItem.quantity--;

        if(selectedItem.quantity <= 0 ) 
        {
            if (uiSlot[selectedItemIndex].equipped) 
            {
                UnEquip(selectedItemIndex);
            }

            selectedItem.itemData = null;
            ClearSelectedItemWindow();
        }
    }

    private void RemoveItem(ItemData item)
    {
        throw new NotImplementedException();
    }

    private void HasItem(ItemData item, int quantity)
    {
        throw new NotImplementedException();
    }
}
