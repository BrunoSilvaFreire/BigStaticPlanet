using System;
using Lunari.Tsuki;
using Lunari.Tsuki.Entities;
using UnityEngine;

namespace Game
{
    public class Skin : Trait
    {
        private Animator _animator;
        private GameObject _activeSkin;
        private Rigidbody2D _rigidBody;
        [SerializeField] private string _velocityField = "Velocity";
        [SerializeField] private GameObject _defaultSkin;

        public void ChangeSkin(GameObject otherPrefab)
        {
            _activeSkin = otherPrefab.Clone(transform);
            _animator = _activeSkin.GetComponentInChildren<Animator>();
        }

        private void Start()
        {
            ChangeSkin(_defaultSkin);
        }

        private void Update()
        {
            _animator.SetFloat(_velocityField, _rigidBody.velocity.magnitude);
        }

        public override void Describe(TraitDescriptor descriptor)
        {
            descriptor.RequiresComponent(out _rigidBody);
        }
    }
}