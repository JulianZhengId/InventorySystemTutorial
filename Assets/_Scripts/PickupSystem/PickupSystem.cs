using InventorySystem.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupSystem : MonoBehaviour
{
    [SerializeField] private InventoryScrObj inventoryScrObj;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PickableItem pickableItem = collision.GetComponent<PickableItem>();

        if (pickableItem != null)
        {
            int remainder = inventoryScrObj.AddItem(pickableItem.InventoryItem, pickableItem.Quantity);

            if (remainder == 0) pickableItem.DestroyItem();
            else pickableItem.Quantity = remainder;

        }
    }
}
