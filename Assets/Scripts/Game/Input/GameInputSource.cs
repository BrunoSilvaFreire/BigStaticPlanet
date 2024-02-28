using Lunari.Tsuki2D.Runtime.Input.TopDown;
using UnityEngine.InputSystem;

namespace Game.Input
{
    public class GameInputSource : TopDownInputSource
    {
        private InputAction _inventory;
        private InputAction _interact;
        public string inventoryActionName = "Inventory";
        public string interactActionName = "Interact";

        protected override void Start()
        {
            base.Start();
            _inventory = input.actions[inventoryActionName];
            _interact = input.actions[interactActionName];
        }

        public bool GetInventory()
        {
            return _inventory.IsPressed();
        }

        public bool GetInteract()
        {
            return _interact.IsPressed();
        }
    }
}