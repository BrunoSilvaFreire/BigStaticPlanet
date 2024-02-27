using UnityEngine;
namespace Lunari.Tsuki2D.Runtime.Movement.Updaters {
    public class StandaloneRaycastUpdater : AbstractRaycastUpdater {
        public LayerMask layerMask;

        protected override LayerMask SelectLayerMask() {
            return layerMask;
        }
    }
}