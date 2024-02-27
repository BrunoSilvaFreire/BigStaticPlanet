using UnityEngine;
namespace Lunari.Tsuki2D.Runtime.Movement.Attachments {
    public class ParticleEmissionAttachment : MotorAttachment {
        public ParticleSystem system;
        public bool withChildren = true;
        public override void Begin(Motor motor, ref Vector2 velocity) {
            system.Play(withChildren);
        }
        public override void End(Motor motor, ref Vector2 velocity) {
            system.Stop(true);
        }
    }
}