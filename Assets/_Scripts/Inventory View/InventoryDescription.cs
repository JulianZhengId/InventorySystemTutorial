using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace InventorySystem.View
{
    public class InventoryDescription : MonoBehaviour
    {
        [SerializeField] private Image itemImage;
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text descText;

        private void Awake()
        {
            ResetDescription();
        }

        public void ResetDescription()
        {
            itemImage.gameObject.SetActive(false);
            titleText.text = "";
            descText.text = "";
        }

        public void SetDescription(Sprite sprite, string title, string desc)
        {
            itemImage.gameObject.SetActive(true);
            itemImage.sprite = sprite;
            titleText.text = title;
            descText.text = desc;
        }
    }
}