using Lunari.Tsuki.Singletons;
using UnityEngine;

namespace Game
{
    public class Player : Singleton<Player>
    {
        [SerializeField] private TransactionView _transactionView;

        public TransactionView TransactionView => _transactionView;
    }
}