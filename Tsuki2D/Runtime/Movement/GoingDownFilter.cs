using System;
using Lunari.Tsuki2D.Common;
using UnityEngine;
namespace Lunari.Tsuki2D.Runtime.Movement {
    [Serializable]
    public class GoingDownFilter : Filter {
        public override bool Allowed(Collider2D collider2D) {
            return collider2D.attachedRigidbody.velocity.y < 0;
        }
    }
}