using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu]
    public class ItemDefinition : ScriptableObject
    {
        [SerializeField] private Sprite _thumbnail;
        [SerializeField] private uint _price;

        public Sprite Thumbnail => _thumbnail;
        public uint Price => _price;

        public ItemStack CreateItem(byte quantity)
        {
            return new ItemStack(this, quantity);
        }
    }

    [Serializable]
    public class ItemOutput
    {
        public int Into;
        public byte Amount;

        public ItemOutput(int into, byte amount)
        {
            Into = into;
            Amount = amount;
        }
    }

    [Serializable]
    public class ItemMapping
    {
        public ItemStack Item;
        public List<ItemOutput> Outputs = new List<ItemOutput>();

        public ItemMapping(ItemStack item)
        {
            Item = item;
        }
    }

    [Serializable]
    public struct ItemStack
    {
        public ItemDefinition Definition { get; private set; }
        public byte Quantity { get; private set; }

        public ItemStack(ItemDefinition definition, byte quantity)
        {
            Definition = definition;
            Quantity = quantity;
        }

        public ItemStack Clone()
        {
            return new ItemStack(Definition, Quantity);
        }

        public bool IsSimilar(ItemStack other)
        {
            return Definition == other.Definition;
        }

        public void AddQuantity(byte amount)
        {
            Quantity += amount;
        }

        public void SubtractQuantity(byte amount)
        {
            Quantity -= amount;
        }

        public bool IsEmpty()
        {
            return Quantity == 0;
        }
    }
}