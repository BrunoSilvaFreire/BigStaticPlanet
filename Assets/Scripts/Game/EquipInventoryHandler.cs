using Lunari.Tsuki.Entities;

namespace Game
{
    public class EquipInventoryHandler : InventoryHandler
    {
        public override void OnClicked(ItemStack stack)
        {
            var skin = stack.Definition.Skin;
            if (skin == null)
            {
                return;
            }

            if (Player.Instance.Entity.Access(out Skin skinTrait))
            {
                skinTrait.ChangeSkin(skin);
            }
            return;
        }
    }
}