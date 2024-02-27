using UnityEngine;
using UnityEngine.InputSystem;
namespace Lunari.Tsuki2D.Platforming.Input {
    public class InputSystemPlatformerSource : PlatformerInputSource {
        private InputAction jump;
        private InputAction horizontal;
        [SerializeField]
        protected PlayerInput input;
        public string jumpInputName = "Jump";
        public string horizontalInputName = "Horizontal";

        protected virtual void Start() {
            if (input != null) {
                AttachToCurrentInput();
            }
        }

        public PlayerInput Input {
            get => input;
            set {
                if (input == value) {
                    return;
                }
                input = value;
                AttachToCurrentInput();
            }
        }

        private void AttachToCurrentInput() {
            jump = input.actions[jumpInputName];
            horizontal = input.actions[horizontalInputName];
        }

        public override float GetHorizontal() {
            if (horizontal == null) {
                return 0;
            }
            return horizontal.ReadValue<float>();
        }
        public override bool GetJump() {
            if (jump == null) {
                return false;
            }
            return jump.triggered || jump.inProgress;
        }
    }
}