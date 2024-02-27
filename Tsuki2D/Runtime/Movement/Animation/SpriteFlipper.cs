using System;
using Lunari.Tsuki2D.Runtime.Movement.Attachments;
using UnityEngine;
namespace Lunari.Tsuki2D.Runtime.Movement.Animation {
    public class SpriteFlipper : MotorAttachment {
        public new SpriteRenderer renderer;
        public bool facesRight = true;
        public override void Describe(MotorComponentDescriptor descriptor) {
            descriptor.RequiresComponent(out renderer);
        }
        public override void Tick(Motor motor, ref Vector2 velocity) {
            renderer.flipX = facesRight ? velocity.x > 0 : velocity.x < 0;
        }
    }
}