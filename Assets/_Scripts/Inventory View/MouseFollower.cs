using UnityEngine;

namespace InventorySystem.View
{
    public class MouseFollower : MonoBehaviour
    {
        [SerializeField] private Canvas canvas;
        [SerializeField] private InventoryItemUI selectedItem;

        private void Awake()
        {
            canvas = transform.root.GetComponent<Canvas>();
            selectedItem = GetComponentInChildren<InventoryItemUI>();
            Toggle(false);
        }

        public void SetData(Sprite sprite, int amount)
        {
            selectedItem.SetData(sprite, amount);
        }

        void LateUpdate()
        {
            Vector2 position;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                (RectTransform)canvas.transform,
                Input.mousePosition,
                canvas.worldCamera,
                out position);

            transform.position = canvas.transform.TransformPoint(position);
        }

        public void Toggle(bool b)
        {
            gameObject.SetActive(b);
        }
    }
}