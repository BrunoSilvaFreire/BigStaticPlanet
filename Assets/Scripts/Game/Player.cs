﻿using Lunari.Tsuki.Entities;
using Lunari.Tsuki.Singletons;
using UnityEngine;

namespace Game
{
    public class Player : Singleton<Player>
    {
        [SerializeField] private TransactionView _transactionView;
        [SerializeField] private Entity _entity;

        public TransactionView TransactionView => _transactionView;

        public Entity Entity
        {
            get => _entity;
            set => _entity = value;
        }
    }
}