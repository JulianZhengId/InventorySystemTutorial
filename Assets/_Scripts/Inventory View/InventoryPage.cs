using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InventorySystem.View
{
    public class InventoryPage : MonoBehaviour
    {
        [SerializeField] InventoryItemUI itemUIPrefab;
        [SerializeField] RectTransform contentPanel;

        List<InventoryItemUI> listofItemUIs = new List<InventoryItemUI>();

        [SerializeField] private InventoryDescription itemDescription;

        [SerializeField] private MouseFollower mouseFollower;

        private int currentlyDraggedItemIndex = -1;

        public event Action<int> OnDescRequested, OnItemActionRequested, OnStartDragging;

        public event Action<int, int> OnSwapItems;

        private void Awake()
        {
            Hide();
            mouseFollower.Toggle(false);
            itemDescription.ResetDescription();
        }

        public void InitializeInventoryUI(int inventorySize)
        {
            for (int i = 0; i < inventorySize; i++)
            {
                InventoryItemUI itemUI = Instantiate(itemUIPrefab, Vector3.zero, Quaternion.identity);
                itemUI.transform.SetParent(contentPanel);
                listofItemUIs.Add(itemUI);

                //subscriptions to the events
                itemUI.OnItemClicked += HandleItemSelection;
                itemUI.OnItemBeginDrag += HandleBeginDrag;
                itemUI.OnItemDroppedOn += HandleSwap;
                itemUI.OnItemEndDrag += HandleEndDrag;
                itemUI.OnRightMouseBtnClick += HandleShowItemActions;
            }
        }

        internal void ResetAllItems()
        {
            foreach (var item in listofItemUIs)
            {
                item.ResetData();
                item.Deselect();
            }
        }

        internal void UpdateDesc(int itemIndex, Sprite itemImage, string name, string description)
        {
            itemDescription.SetDescription(itemImage, name, description);
            DeselectAllItems();
            listofItemUIs[itemIndex].Select();
        }

        public void UpdateData(int index, Sprite sprite, int amount)
        {
            if (index < listofItemUIs.Count)
            {
                listofItemUIs[index].SetData(sprite, amount);
            }
        }

        private void HandleShowItemActions(InventoryItemUI itemUI)
        {
            int index = listofItemUIs.IndexOf(itemUI);
            if (index == -1) return;
            OnItemActionRequested?.Invoke(index);
        }

        private void HandleEndDrag(InventoryItemUI itemUI)
        {
            ResetDraggedItem();
        }

        private void HandleSwap(InventoryItemUI itemUI)
        {
            int index = listofItemUIs.IndexOf(itemUI);
            if (index == -1) return;

            OnSwapItems?.Invoke(currentlyDraggedItemIndex, index);
            HandleItemSelection(itemUI);
        }

        private void ResetDraggedItem()
        {
            mouseFollower.Toggle(false);
            currentlyDraggedItemIndex = -1;
        }

        private void HandleBeginDrag(InventoryItemUI itemUI)
        {
            int index = listofItemUIs.IndexOf(itemUI);
            if (index == -1) return;

            currentlyDraggedItemIndex = index;
            HandleItemSelection(itemUI);
            OnStartDragging?.Invoke(index);
        }

        public void CreateDraggedItem(Sprite sprite, int amount)
        {
            mouseFollower.SetData(sprite, amount);
            mouseFollower.Toggle(true);
        }

        private void HandleItemSelection(InventoryItemUI itemUI)
        {
            int index = listofItemUIs.IndexOf(itemUI);
            if (index == -1) return;

            OnDescRequested?.Invoke(index);
        }

        public void Show()
        {
            gameObject.SetActive(true);
            ResetSelection();
        }

        public void ResetSelection()
        {
            itemDescription.ResetDescription();
            DeselectAllItems();
            currentlyDraggedItemIndex = -1;
        }

        private void DeselectAllItems()
        {
            foreach (InventoryItemUI itemUI in listofItemUIs)
            {
                itemUI.Deselect();
            }
        }

        public void Hide()
        {
            gameObject.SetActive(false);
            ResetDraggedItem();
        }
    }
}