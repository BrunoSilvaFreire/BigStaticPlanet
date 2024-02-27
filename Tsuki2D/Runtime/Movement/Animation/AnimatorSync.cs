using UnityEngine;
namespace Lunari.Tsuki2D.Runtime.Movement.Animation {
    public abstract class AnimatorSync {
        protected int _hash;
        protected AnimatorSync(string property) {
            Property = property;
            _hash = Animator.StringToHash(property);
        }

        public string Property {
            get;
        }

        public int Hash => _hash;

        public abstract void Update(Animator animator, Motor motor);

        public abstract AnimatorControllerParameterType ParameterType {
            get;
        }
    }

    public delegate T AnimatorSyncGetter<T>(Motor motor);

    public abstract class AnimatorSync<T> : AnimatorSync {
        private readonly AnimatorSyncGetter<T> _getter;
        protected abstract void Set(Animator animator, T value);

        protected AnimatorSync(string property, AnimatorSyncGetter<T> getter) : base(property) {
            _getter = getter;
        }
        public sealed override void Update(Animator animator, Motor motor) {
            Set(animator, _getter(motor));
        }
    }

    public class FloatAnimatorSync : AnimatorSync<float> {
        public FloatAnimatorSync(string property, AnimatorSyncGetter<float> getter) : base(property, getter) {
        }

        protected override void Set(Animator animator, float value) {
            animator.SetFloat(_hash, value);
        }

        public override AnimatorControllerParameterType ParameterType => AnimatorControllerParameterType.Float;
    }
    public class IntAnimatorSync : AnimatorSync<int> {
        public IntAnimatorSync(string property, AnimatorSyncGetter<int> getter) : base(property, getter) {
        }

        protected override void Set(Animator animator, int value) {
            animator.SetInteger(_hash, value);
        }

        public override AnimatorControllerParameterType ParameterType => AnimatorControllerParameterType.Int;
    }

    public class BoolAnimatorSync : AnimatorSync<bool> {
        public BoolAnimatorSync(string property, AnimatorSyncGetter<bool> getter) : base(property, getter) {
        }

        protected override void Set(Animator animator, bool value) {
            animator.SetBool(_hash, value);
        }

        public override AnimatorControllerParameterType ParameterType => AnimatorControllerParameterType.Bool;
    }
}