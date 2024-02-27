using Lunari.Tsuki.Entities;

namespace Game
{
    public class ShopKeeper :  Trait
    {
        private Inventory _inventory;
        private Interactable _interactable;
        public override void Describe(TraitDescriptor descriptor)
        {
            if (descriptor.DependsOn(out _inventory, out _interactable))
            {
                _interactable.OnInteract(Attend);
            }
        }

        private void Attend(Entity entity)
        {
            
        }
    }
}