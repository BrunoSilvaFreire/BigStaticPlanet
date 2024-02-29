using System;
using Game.UI;
using Game.UI.Datenshi.UI;
using UnityEngine;

namespace Game
{
    public class TransactionView : AnimatedView
    {
        [SerializeField] private InventoryView _inventory;
        [SerializeField] private InventoryView _otherView;
        [SerializeField] private Inventory _ownInventory;

        public void SetOtherInventory(Inventory inventory)
        {
            _inventory.Replicate(_ownInventory);
            _otherView.Replicate(inventory);
        }


        protected override void Conceal()
        {
            base.Conceal();
            _inventory.SetShown(false, false);
            _otherView.SetShown(false, false);
        }

        protected override void Reveal()
        {
            base.Reveal();
            _inventory.SetShown(true, true);
            _otherView.SetShown(true, true);
        }

        protected override void ImmediateConceal()
        {
            base.ImmediateConceal();
            _inventory.SetShown(false, true);
            _otherView.SetShown(false, true);
        }

        protected override void ImmediateReveal()
        {
            base.ImmediateReveal();
            _inventory.SetShown(true, true);
            _otherView.SetShown(true, true);
        }
    }
}