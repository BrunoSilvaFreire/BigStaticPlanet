using Lunari.Tsuki2D.Runtime.Input.TopDown;
using UnityEngine.InputSystem;

namespace Game.Input
{
    public class GameInputSource : TopDownInputSource
    {
        private InputAction _inventory;
        private InputAction _back;
        private InputAction _interact;
        public string inventoryActionName = "Inventory";
        public string interactActionName = "Interact";
        public string backActionName = "Cancel";

        protected override void Start()
        {
            base.Start();
            _inventory = input.actions[inventoryActionName];
            _interact = input.actions[interactActionName];
            _back = input.actions[backActionName];
        }

        public void SetInteractInputEnabled(bool isEnabled)
        {
            if (isEnabled)
            {
                _interact.Enable();
            }
            else
            {
                _interact.Disable();
            }
        }

        public void SetInventoryInputEnabled(bool isEnabled)
        {
            if (isEnabled)
            {
                _inventory.Enable();
            }
            else
            {
                _inventory.Disable();
            }
        }

        public void SetMovementInputEnabled(bool isEnabled)
        {
            if (isEnabled)
            {
                this.horizontal.Enable();
                this.vertical.Enable();
            }
            else
            {
                this.horizontal.Disable();
                this.vertical.Disable();
            }
        }

        public bool GetInventory()
        {
            return _inventory.IsPressed();
        }

        public bool GetInteract()
        {
            return _interact.IsPressed();
        }

        public bool GetBack()
        {
            return _back.IsPressed();
        }
    }
}