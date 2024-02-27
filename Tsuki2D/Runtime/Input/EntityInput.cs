using Lunari.Tsuki.Entities;
using UnityEngine;
#if TSUKI_ENTITIES
#endif

namespace Lunari.Tsuki2D.Runtime.Input {
#if TSUKI_ENTITIES
    public abstract class EntityInput : Trait {
#else
    public abstract class EntityInput : MonoBehaviour {
#endif
        protected abstract void Clear();
    }

    public abstract class EntityInput<S> : EntityInput where S : InputSource {
        [SerializeField]
        private S source;
#if TSUKI_ENTITIES
        public override void Describe(TraitDescriptor descriptor) {
            base.Describe(descriptor);
        }
#endif
        private void Update() {
            var src = source;
            if (src == null) {
                Clear();
            } else {
                ReadInput(src);
            }
        }

        protected abstract void ReadInput(S src);
    }
}