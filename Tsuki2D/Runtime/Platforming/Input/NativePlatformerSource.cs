namespace Lunari.Tsuki2D.Platforming.Input {
    public sealed class NativePlatformerSource : PlatformerInputSource {

        public override float GetHorizontal() {
            return UnityEngine.Input.GetAxis("Horizontal");
        }
        public override bool GetJump() {
            return UnityEngine.Input.GetButtonDown("Jump");
        }
    }
}