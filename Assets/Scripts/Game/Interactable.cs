using Lunari.Tsuki.Entities;
using UnityEngine;
using UnityEngine.Events;

namespace Game
{
    public class Interactable : Trait
    {
        [SerializeField] private EntityEvent _onInteract;

        public void InteractWith(Entity interactee)
        {
            _onInteract.Invoke(interactee);
        }

        public void OnInteract(UnityAction<Entity> other)
        {
            _onInteract.AddListener(other);
        }
    }
}