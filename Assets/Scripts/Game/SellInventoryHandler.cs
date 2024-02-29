using Lunari.Tsuki.Entities;
using UnityEngine;

namespace Game
{
    public class SellInventoryHandler : InventoryHandler
    {
        [SerializeField]
        private Inventory _shopkeeperInventory;

        public override void OnClicked(ItemStack stack)
        {
            if ( stack.Definition == null || _shopkeeperInventory == null)
            {
                return;
            }
            var player = Player.Instance;
            if (!player.Entity.Access(out Inventory inventory))
            {
                return;
            }
            var sellValue = stack.Definition.Price;
            if (!inventory.RemoveItem(stack))
            {
                return;
            }

            if (!_shopkeeperInventory.AddItem(stack))
            {
                inventory.AddItem(stack);
                return;
            }

            Player.Instance.Money += sellValue;
            return;
        }
    }
}