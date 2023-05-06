using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace InventorySystem.Model
{
    [CreateAssetMenu]
    public class InventoryScrObj : ScriptableObject
    {
        [SerializeField] private List<InventoryItem> itemsInventory;

        [field: SerializeField]
        public int Size { get; private set; } = 10;

        public event Action<Dictionary<int, InventoryItem>> OnInventoryUpdated;

        public void Initialize()
        {
            itemsInventory = new List<InventoryItem>();
            for (int i = 0; i < Size; i++)
            {
                itemsInventory.Add(InventoryItem.GetEmptyItem());
            }
        }

        public int AddItem(ItemScrObj itemScrObj, int amount)
        {
            //add non-stackable items
            if (!itemScrObj.IsStackable)
            {
                while (amount > 0 && !IsInventoryFull())
                {
                    amount -= AddItemsToFirstEmptySlot(itemScrObj, 1);
                }
                InformAboutChange();
                return amount;
            }

            //add stackable items
            amount = AddStackableItem(itemScrObj, amount);
            InformAboutChange();
            return amount;
        }

        private int AddItemsToFirstEmptySlot(ItemScrObj itemScrObj, int amount)
        {
            for (int i = 0; i < itemsInventory.Count; i++)
            {
                if (itemsInventory[i].IsEmpty)
                {
                    itemsInventory[i] = new InventoryItem { itemScrObj = itemScrObj, amount = amount };
                    return amount;
                }
            }

            return 0;
        }

        private bool IsInventoryFull() => !itemsInventory.Where(item => item.IsEmpty).Any();

        private int AddStackableItem(ItemScrObj itemScrObj, int amount)
        {
            for (int i = 0; i < itemsInventory.Count; i++)
            {
                if (itemsInventory[i].IsEmpty) continue;

                if (itemsInventory[i].itemScrObj.ID == itemScrObj.ID)
                {
                    int possibleAmountToTake = itemsInventory[i].itemScrObj.MaxStackSize - itemsInventory[i].amount;

                    if (amount > possibleAmountToTake)
                    {
                        itemsInventory[i] = itemsInventory[i].ChangeQuantity(itemsInventory[i].itemScrObj.MaxStackSize);
                        amount -= possibleAmountToTake;
                    }
                    else
                    {
                        itemsInventory[i] = itemsInventory[i].ChangeQuantity(amount + itemsInventory[i].amount);
                        InformAboutChange();
                        return 0;
                    }
                }
            }

            while (amount > 0 && !IsInventoryFull())
            {
                int newItemAmount = Mathf.Clamp(amount, 0, itemScrObj.MaxStackSize);
                amount -= newItemAmount;
                AddItemsToFirstEmptySlot(itemScrObj, newItemAmount);
            }

            return amount;
        }

        public InventoryItem GetItemAt(int itemIndex)
        {
            return itemsInventory[itemIndex];
        }

        public void SwapItems(int itemIndex1, int itemIndex2)
        {
            InventoryItem item1 = itemsInventory[itemIndex1];
            itemsInventory[itemIndex1] = itemsInventory[itemIndex2];
            itemsInventory[itemIndex2] = item1;

            InformAboutChange();
        }

        private void InformAboutChange()
        {
            OnInventoryUpdated?.Invoke(GetCurrentInventoryState());
        }

        //we dont want inventory controller to grab of the list and modify it (not secure)
        //therefore we create a public dictionary and give it to inventory controller
        //we also dont want to update the list fully because some of the items might be empty
        //we also dont want to give the original list of inventory items, therefore we give a duplicate as a dictionary
        //this will make it more secure
        public Dictionary<int, InventoryItem> GetCurrentInventoryState()
        {
            Dictionary<int, InventoryItem> returnValue = new Dictionary<int, InventoryItem>();

            for (int i = 0; i < itemsInventory.Count; i++)
            {
                if (itemsInventory[i].IsEmpty) continue;
                returnValue[i] = itemsInventory[i];
            }

            return returnValue;
        }
    }

    [Serializable]
    public struct InventoryItem
    {
        public int amount;
        public ItemScrObj itemScrObj;

        public bool IsEmpty => itemScrObj == null;

        public InventoryItem ChangeQuantity(int newQuantity)
        {
            return new InventoryItem
            {
                amount = newQuantity,
                itemScrObj = this.itemScrObj
            };
        }

        public static InventoryItem GetEmptyItem()
        {
            return new InventoryItem
            {
                amount = 0,
                itemScrObj = null
            };
        }
    }
}
