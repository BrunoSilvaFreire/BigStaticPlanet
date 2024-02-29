using System;
using Game.UI.Datenshi.UI;
using Lunari.Tsuki;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.UI
{
    public class InventoryView : AnimatedView
    {
        [SerializeField] private Transform _inventoryContainer;
        [SerializeField] private ItemStackView _itemStackViewPrefab;
        private ItemStackView[] _views;
        private Inventory _inventory;
        [SerializeField] private InventoryHandler _handler;

        private void Awake()
        {
            AllocateViews();
        }

        private void Start()
        {
            ImmediateConceal();
        }

        private void AllocateViews()
        {
            _views = new ItemStackView[Inventory.InventorySize];
            for (var i = 0; i < Inventory.InventorySize; i++)
            {
                var view = _itemStackViewPrefab.Clone(_inventoryContainer);
                view.ClickCallback = OnClicked;
                _views[i] = view;
            }

        }

        private void OnClicked(ItemStack itemStack)
        {
            if (_inventory == null)
            {
                return;
            }

            if (_handler == null)
            {
                return;
            }

            _handler.OnClicked(itemStack);
        }

        public void Replicate(Inventory inventory)
        {
            if (inventory != _inventory)
            {
                if (_inventory != null)
                {
                    _inventory.OnChanged.RemoveListener(OnChanged);
                }

                if (inventory != null)
                {
                    inventory.OnChanged.AddListener(OnChanged);
                }
            }
            _inventory = inventory;
            for (var i = 0; i < inventory.Contents.Count; i++)
            {
                var content = inventory.Contents[i];
                _views[i].Replicate(content);
            }
        }

        private void OnChanged()
        {
            Replicate(_inventory);
        }
    }
}