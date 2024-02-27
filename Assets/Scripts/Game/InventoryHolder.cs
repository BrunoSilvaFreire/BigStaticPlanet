using Lunari.Tsuki.Entities;

namespace Game
{
    public class InventoryHolder : Trait
    {
        private const int kInventorySize = 64;
        private ItemStack[] _inventory;
    }
}