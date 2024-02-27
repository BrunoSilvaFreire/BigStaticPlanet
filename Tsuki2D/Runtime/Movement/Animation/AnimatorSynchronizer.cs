using System;
using System.Collections.Generic;
using Lunari.Tsuki2D.Runtime.Movement.Attachments;
using UnityEngine;
namespace Lunari.Tsuki2D.Runtime.Movement.Animation {
    public class AnimatorSynchronizer : MotorAttachment {
        private Animator _animator;
        private readonly List<AnimatorSync> _synchronizers = new List<AnimatorSync>();
        public override void Describe(MotorComponentDescriptor descriptor) {
            descriptor.RequiresComponent(out _animator);
        }
        public void Add(AnimatorSync sync) {
            _synchronizers.Add(sync);
        }
        public void Remove(AnimatorSync sync) {
            _synchronizers.Remove(sync);
        }
        public override void Tick(Motor motor, ref Vector2 velocity) {
            foreach (var synchronizer in _synchronizers) {
                synchronizer.Update(_animator, motor);
            }
        }
    }
}