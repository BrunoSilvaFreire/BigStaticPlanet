using System;
using System.Collections.Generic;
using System.Linq;
using Lunari.Tsuki.Entities;
using Unity.Services.Analytics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game
{
    public class InventoryTransaction
    {
        private List<ItemMapping> mappings = new List<ItemMapping>();
        private List<ItemStack> leftovers = new List<ItemStack>();
        private Inventory inventory;

        public InventoryTransaction(IEnumerable<ItemStack> sources, Inventory inventory)
        {
            this.inventory = inventory;

            // Initialize mappings and leftovers from sources
            var contents = inventory.Contents;
            foreach (var source in sources)
            {
                var mapping = new ItemMapping(source.Clone());
                mappings.Add(mapping);

                // Attempt to map items to existing inventory slots or determine leftovers
                var fullyMapped = false;

                // Attempt to add to existing slots
                for (var i = 0; i < contents.Count; i++)
                {
                    if (!contents[i].IsSimilar(source) || contents[i].Quantity >= byte.MaxValue)
                    {
                        continue;
                    }

                    var availableToAdd = (byte)(byte.MaxValue - contents[i].Quantity);
                    var toAdd = (byte)Math.Min(availableToAdd, source.Quantity);
                    mapping.Outputs.Add(new ItemOutput(i, toAdd));
                    source.SubtractQuantity(toAdd);

                    if (!source.IsEmpty())
                    {
                        continue;
                    }

                    fullyMapped = true;
                    break; // Break if fully mapped
                }

                // If not fully mapped, attempt to find empty slots
                if (!fullyMapped)
                {
                    for (var i = 0; i < contents.Count; i++)
                    {
                        if (!contents[i].IsEmpty())
                        {
                            continue;
                        }

                        mapping.Outputs.Add(new ItemOutput(i, source.Quantity));
                        fullyMapped = true;
                        break; // Break if item has been fully placed
                    }
                }

                if (!fullyMapped)
                {
                    leftovers.Add(source); // Add to leftovers if not fully placed
                }
            }
        }

        public bool CanFullyApply()
        {
            return leftovers.Count == 0;
        }

        public bool TryApply(bool mustApplyAll = true)
        {
            if (mustApplyAll && !CanFullyApply()) return false;

            foreach (var mapping in mappings)
            {
                foreach (var output in mapping.Outputs)
                {
                    if (inventory.Contents[output.Into].IsEmpty() ||
                        inventory.Contents[output.Into].IsSimilar(mapping.Item))
                    {
                        inventory.Contents[output.Into].AddQuantity(output.Amount);
                    }
                }
            }

            return true;
        }
    }

    public class Inventory : Trait
    {
        private ItemStack[] _contents;
        [SerializeField] private bool _generateRandomContentsFromRegistry;
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
                        return true;
                    }
                }
            }

            return false; // Item not found or not enough quantity
        }
    }
}