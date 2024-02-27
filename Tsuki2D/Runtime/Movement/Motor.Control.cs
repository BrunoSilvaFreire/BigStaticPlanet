using Lunari.Tsuki.Exceptions;
using UnityEngine;
namespace Lunari.Tsuki2D.Runtime.Movement {
    public partial class Motor {
        [SerializeField]
        private float control = 1;

        [SerializeField]
        private float leftControl = 1;

        [SerializeField]
        private float rightControl = 1;

        public float Control {
            get => control;
            set => control = value;
        }

        public float LeftControl {
            get => leftControl;
            set => leftControl = value;
        }

        public float RightControl {
            get => rightControl;
            set => rightControl = value;
        }

        public ref float GetDirectionControlReference(int direction) {
            switch (direction) {
                case -1:
                    return ref leftControl;
                case 1:
                    return ref rightControl;
            }

            throw new WTFException("Last direction is 0");
        }

        public float GetDirectionControl(int direction) {
            switch (direction) {
                case -1:
                    return leftControl * control;
                case 1:
                    return rightControl * control;
            }

            return control;
        }

        public void SetDirectionControl(int direction, float evaluate) {
            switch (direction) {
                case -1:
                    leftControl = evaluate;
                    break;
                case 1:
                    rightControl = evaluate;
                    break;
            }
        }
    }
}