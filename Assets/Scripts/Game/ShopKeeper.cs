using Lunari.Tsuki.Entities;

namespace Game
{
    public class ShopKeeper :  Trait
    {
        private InventoryHolder _inventoryHolder;
        private Interactable _interactable;
        public override void Describe(TraitDescriptor descriptor)
        {
            if (descriptor.DependsOn(out _inventoryHolder, out _interactable))
            {
                _interactable.OnInteract(Attend);
            }
        }

        private void Attend(Entity entity)
        {
            
        }
    }
}