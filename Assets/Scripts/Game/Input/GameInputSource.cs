using Lunari.Tsuki2D.Runtime.Input.TopDown;
using UnityEngine.InputSystem;

namespace Game.Input
{
    public class GameInputSource : TopDownInputSource
    {
        private InputAction _action;
        public string inventoryActionName = "Inventory";

        protected override void Start()
        {
            base.Start();
            _action = input.actions[inventoryActionName];
        }

        public bool GetInventory()
        {
            return _action.IsPressed();
        }
    }
}