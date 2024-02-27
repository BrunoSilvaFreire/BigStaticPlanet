using System;
using UnityEngine;

namespace Game
{
    public class ItemDefinition : ScriptableObject
    {
        [SerializeField]
        private Sprite _thumbnail;

        public Sprite Thumbnail => _thumbnail;
    }

    [Serializable]
    public struct ItemStack // Minecraft anyone?
    {
        public ItemDefinition Definition { get; }

        public byte Quantity { get; }
    }
}