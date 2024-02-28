using System;
using System.Numerics;
using Game.Input;
using Game.UI;
using Lunari.Tsuki;
using Lunari.Tsuki.Entities;
using Lunari.Tsuki2D.Runtime.Movement;
using Lunari.Tsuki2D.Runtime.Movement.Attachments;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

namespace Game
{
    public class InteractionAttachment : MotorAttachmentWithInput<GameInput>
    {
        [SerializeField] private float _interactionRadius;
        [SerializeField] private LayerMask _interactionLayerMask;
        [SerializeField] private View _interactView;
        [SerializeField] private Vector3 _viewOffset;
        private readonly Collider2D[] _interactibles = new Collider2D[4];
        private Interactable _selectedInteractable;

        private void OnDrawGizmos()
        {
            Gizmos2.DrawWireCircle2D(transform.position, _interactionRadius, Color.red);
        }

        Interactable FindClosest(int totalNumInteractibles)
        {
            var closest = float.MaxValue;
            Interactable candidate = null;
            var pos = transform.position;
            for (var i = 0; i < totalNumInteractibles; i++)
            {
                if (!_interactibles[i].FindEntity(out Entity entity))
                {
                    continue;
                }

                if (!entity.Access(out Interactable interactable))
                {
                    continue;
                }

                var distance = Vector2.Distance(pos, entity.transform.position);
                if (distance > closest)
                {
                    continue;
                }

                closest = distance;
                candidate = interactable;
            }

            return candidate;
        }

        private void Update()
        {
            if (_selectedInteractable == null)
            {
                return;
            }

            _interactView.transform.position = _selectedInteractable.transform.position + _viewOffset;
        }

        public override void Tick(Motor motor, GameInput input, ref Vector2 velocity)
        {
            var num = Physics2D.OverlapCircleNonAlloc(
                transform.position,
                _interactionRadius,
                _interactibles,
                _interactionLayerMask
            );
            var hasInteractablesNearby = num > 0;
            _interactView.Shown = hasInteractablesNearby;

            if (!hasInteractablesNearby)
            {
                _selectedInteractable = null;
                return;
            }

            var interacted = input.Interact.Consume();
            _selectedInteractable = FindClosest(num);
            if (interacted)
            {
                _selectedInteractable.InteractWith(motor.Owner);
            }
        }
    }
}