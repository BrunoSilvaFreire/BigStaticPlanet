using Game.Input;
using Game.UI;
using Lunari.Tsuki2D.Runtime.Movement;
using Lunari.Tsuki2D.Runtime.Movement.Attachments;
using UnityEngine;

namespace Game.Movement
{
    public class InventoryViewAttachment : MotorAttachmentWithInput<GameInput>
    {
        [SerializeField]
        private InventoryView _view;

        [SerializeField] private Inventory _inventory;

        public override void Tick(Motor motor, GameInput input, ref Vector2 velocity)
        {
            if (!input.Inventory.Consume())
            {
                return;
            }
            _view.Replicate(_inventory);
            _view.Shown = !_view.Shown;
        }
    }
}