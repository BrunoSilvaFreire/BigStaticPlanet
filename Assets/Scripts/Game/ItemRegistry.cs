using System.Collections.Generic;
using Lunari.Tsuki.Singletons;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu]
    public class ItemRegistry : ScriptableSingleton<ItemRegistry>
    {
        [SerializeField] private List<ItemDefinition> _items;

        public IReadOnlyList<ItemDefinition> Items => _items;
    }
}