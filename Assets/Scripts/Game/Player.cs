using Lunari.Tsuki.Entities;
using Lunari.Tsuki.Singletons;
using TMPro;
using UnityEngine;

namespace Game
{
    public class Player : Singleton<Player>
    {
        [SerializeField] private TransactionView _transactionView;
        [SerializeField] private Entity _entity;
        [SerializeField] private uint _money = 100000;
        [SerializeField] private TMP_Text _moneyLabel;
        public uint Money
        {
            get => _money;
            set
            {
                _money = value;
                _moneyLabel.text = $"${_money}";
            }
        }

        protected override void Start()
        {
            base.Start();
            _moneyLabel.text = $"${_money}";
        }

        public TransactionView TransactionView => _transactionView;

        public Entity Entity
        {
            get => _entity;
            set => _entity = value;
        }
    }
}