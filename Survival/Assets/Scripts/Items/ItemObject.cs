using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ItemObject : MonoBehaviour, IInteractable
{
    public ItemData itemData;

    public string GetInteractPrompt()
    {
        return string.Format("Pickup {0}", itemData.displayName);
    }

    public void OnInteract()
    {
        Inventory.instance.AddItem(itemData);
        Destroy(gameObject);
    }
}
