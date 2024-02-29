using UnityEngine;
using UnityEngine.InputSystem;
namespace Lunari.Tsuki2D.Runtime.Input.TopDown {
    public class TopDownInputSource : InputSource {
        protected InputAction vertical;
        protected InputAction horizontal;
        public PlayerInput input;
        public string verticalInputName = "vertical";
        public string horizontalInputName = "horizontal";

        protected virtual void Start() {
            if (input == null) {
                Debug.LogWarning("TopDownInputSource does not have a PlayerInput set", this);
                return;
            }
            vertical = input.actions[verticalInputName];
            horizontal = input.actions[horizontalInputName];
        }

        public float GetVertical() {
            return vertical.ReadValue<float>();
        }

        public float GetHorizontal() {
            return horizontal.ReadValue<float>();
        }
    }
}