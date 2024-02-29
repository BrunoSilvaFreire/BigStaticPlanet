using System.Collections.Generic;
using Lunari.Tsuki.Entities;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace Game
{
    public class Inventory : Trait
    {
        private ItemStack[] _contents;
        [SerializeField] private bool _generateRandomContentsFromRegistry;
        [SerializeField] UnityEvent _onChanged;

        public UnityEvent OnChanged => _onChanged;

        public const int InventorySize = 3 * 8;

        private void Awake()
        {
            _contents = new ItemStack[InventorySize];
        }

        private void Start()
        {
            if (_generateRandomContentsFromRegistry)
            {
                PopulateWithRandomItems();
            }
        }

        private void PopulateWithRandomItems()
        {
            var definitions = ItemRegistry.Instance.Items;
            var numItemsToGenerate = Random.Range(1, InventorySize);
            for (var i = 0; i < numItemsToGenerate; i++)
            {
                if (definitions.Count <= 0)
                {
                    continue;
                }

                var randomIndex = Random.Range(1, definitions.Count);
                var randomDefinition = definitions[randomIndex];
                var randomQuantity =
                    Random.Range(1, byte.MaxValue + 1); // Generate a random quantity within valid range
                var itemStack = randomDefinition.CreateItem((byte)randomQuantity);

                AddItem(itemStack);
            }
        }

        public IReadOnlyList<ItemStack> Contents => _contents;

        // Method to find an inventory slot for a given item
        private int FindSlotForItem(ItemStack item)
        {
            for (var i = 0; i < InventorySize; i++)
            {
                if (_contents[i].IsSimilar(item) && _contents[i].Quantity < byte.MaxValue)
                    return i;
                if (_contents[i].IsEmpty())
                    return i;
            }

            return -1; // Inventory is full or no suitable slot
        }

        public bool AddItem(ItemStack itemToAdd)
        {
            var slot = FindSlotForItem(itemToAdd);
            if (slot == -1)
            {
                for (int i = 0; i < _contents.Length; i++)
                {
                    if (_contents[i].IsEmpty())
                    {
                        _contents[i] = itemToAdd;
                        OnChanged.Invoke();
                        return true;
                    }
                }

                return false;
            }
            else
            {
                if (_contents[slot].IsEmpty())
                {
                    _contents[slot] = itemToAdd;
                }
                else
                {
                    _contents[slot].AddQuantity(itemToAdd.Quantity);
                }

                OnChanged.Invoke();
                return true;
            }
        }

        // Method to remove an item (simplified version)
        public bool RemoveItem(ItemStack itemToRemove)
        {
            for (var i = 0; i < InventorySize; i++)
            {
                if (_contents[i].IsSimilar(itemToRemove))
                {
                    if (_contents[i].Quantity >= itemToRemove.Quantity)
                    {
                        _contents[i].SubtractQuantity(itemToRemove.Quantity);
                        OnChanged.Invoke();
                        return true;
                    }
                }
            }

            return false; // Item not found or not enough quantity
        }
    }
}