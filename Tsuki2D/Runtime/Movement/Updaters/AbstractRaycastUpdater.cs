using UnityEngine;
namespace Lunari.Tsuki2D.Runtime.Movement.Updaters {
    public abstract class AbstractRaycastUpdater : SupportStateUpdater {
        public float length = 0.1F;
        private LayerMask cachedLayerMask;

        private void Start() {
            ReloadMask();
        }

        protected abstract LayerMask SelectLayerMask();

        public void ReloadMask() {
            cachedLayerMask = SelectLayerMask();
        }

        private bool Check(Collider2D collider, ContactFilter2D filter, Vector2 direction,
            bool allowEffectors = false) {
            var results = new RaycastHit2D[2];
            var l = collider.Cast(direction, filter, results, length);
            if (allowEffectors) {
                return l > 0;
            }

            for (var i = 0; i < l; i++) {
                var col = results[i];
                if (col.collider.usedByEffector) {
                    continue;
                }

                return true;
            }

            return false;
        }

        public override void Test(Motor motor, ref SupportState state) {
            var filter = new ContactFilter2D {
                useTriggers = false,
                layerMask = cachedLayerMask,
                useLayerMask = true
            };
            var col = motor.collider;
            state.up = Check(col, filter, Vector2.up);
            state.down = Check(col, filter, Vector2.down, true);
            state.left = Check(col, filter, Vector2.left);
            state.right = Check(col, filter, Vector2.right);
        }
    }
}