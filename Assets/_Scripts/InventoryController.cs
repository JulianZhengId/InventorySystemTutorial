using InventorySystem.View;
using InventorySystem.Model;
using UnityEngine;
using System.Collections.Generic;
using System;

namespace InventorySystem
{
    public class InventoryController : MonoBehaviour
    {
        [SerializeField] private InventoryPage inventoryPage;
        [SerializeField] private InventoryScrObj itemsInventoryScrObj;
        public int inventorySize = 15;

        public List<InventoryItem> initialInventoryItems = new List<InventoryItem>();

        private void Start()
        {
            PrepareUI();
            PrepareInventoryData();
        }

        private void PrepareInventoryData()
        {
            itemsInventoryScrObj.Initialize();
            itemsInventoryScrObj.OnInventoryUpdated += UpdateInventoryUI;

            foreach (InventoryItem item in initialInventoryItems)
            {
                if (item.IsEmpty) continue;

                itemsInventoryScrObj.AddItem(item.itemScrObj, item.amount);
            }
        }

        private void UpdateInventoryUI(Dictionary<int, InventoryItem> itemsDict)
        {
            inventoryPage.ResetAllItems();

            foreach (var item in itemsDict)
            {
                inventoryPage.UpdateData(item.Key, item.Value.itemScrObj.ItemImage, item.Value.amount);
            }
        }

        private void PrepareUI()
        {
            inventoryPage.InitializeInventoryUI(itemsInventoryScrObj.Size);
            inventoryPage.OnDescRequested += HandleDescRequest;
            inventoryPage.OnItemActionRequested += HandleItemActionRequest;
            inventoryPage.OnStartDragging += HandleDragging;
            inventoryPage.OnSwapItems += HandleSwapItems;
        }

        private void HandleSwapItems(int itemIndex1, int itemIndex2)
        {
            //change the order in the model
            itemsInventoryScrObj.SwapItems(itemIndex1, itemIndex2);
        }

        private void HandleDragging(int itemIndex)
        {
            InventoryItem inventoryItem = itemsInventoryScrObj.GetItemAt(itemIndex);

            if (inventoryItem.IsEmpty) return;
            inventoryPage.CreateDraggedItem(inventoryItem.itemScrObj.ItemImage, inventoryItem.amount);
        }

        private void HandleItemActionRequest(int obj)
        {

        }

        private void HandleDescRequest(int itemIndex)
        {
            InventoryItem item = itemsInventoryScrObj.GetItemAt(itemIndex);

            if (item.IsEmpty)
            {
                inventoryPage.ResetSelection();
                return;
            }

            ItemScrObj itemScrObj = item.itemScrObj;
            inventoryPage.UpdateDesc(itemIndex, itemScrObj.ItemImage, itemScrObj.Name, itemScrObj.Description);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.I))
            {
                if (inventoryPage.isActiveAndEnabled)
                {
                    inventoryPage.Hide();
                }
                else
                {
                    inventoryPage.Show();

                    foreach (var item in itemsInventoryScrObj.GetCurrentInventoryState())
                    {
                        inventoryPage.UpdateData(item.Key, item.Value.itemScrObj.ItemImage, item.Value.amount);
                    }
                }
            }
        }
    }
}