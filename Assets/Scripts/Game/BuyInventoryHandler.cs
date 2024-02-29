using Lunari.Tsuki.Entities;
using UnityEngine;

namespace Game
{
    public class BuyInventoryHandler : InventoryHandler
    {
        [SerializeField]
        private Inventory _shopkeeperInventory; 

        public override void OnClicked(ItemStack stack)
        {
            if (stack.Definition == null || _shopkeeperInventory == null)
            {
                return;
            }

            var cost = stack.Definition.Price;
            var player = Player.Instance;
            if (player.Money < cost || !_shopkeeperInventory.RemoveItem(stack))
            {
                return;
            }

            if (!player.Entity.Access(out Inventory inventory))
            {
                return;
            }
            if (!inventory.AddItem(stack))
            {
                _shopkeeperInventory.AddItem(stack);
                return;
            }

            player.Money -= cost;
            return;
        }
    }
}