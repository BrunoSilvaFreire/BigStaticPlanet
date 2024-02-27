using Lunari.Tsuki2D.Runtime.Input;
using Lunari.Tsuki2D.Runtime.Input.TopDown;

namespace Game.Input
{
    public class GameInput : TopDownInput
    {
        private void Awake()
        {
            Inventory = new EntityAction();
        }

        public EntityAction Inventory { get; private set; }

        protected override void ReadInput(TopDownInputSource src)
        {
            base.ReadInput(src);
            if (src is GameInputSource gameSrc)
            {
                Inventory.Current = gameSrc.GetInventory();
            }
        }
    }
}