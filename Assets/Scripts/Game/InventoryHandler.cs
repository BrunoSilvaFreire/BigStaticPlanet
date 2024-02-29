using UnityEngine;

namespace Game
{
    public abstract class InventoryHandler : MonoBehaviour
    {
        public abstract void OnClicked(ItemStack stack);
    }
}