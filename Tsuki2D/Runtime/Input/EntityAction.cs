using System;
using Lunari.Tsuki.Misc;
namespace Lunari.Tsuki2D.Runtime.Input {
    /// <summary>
    /// Represents some action that the entity is able to perform.
    /// Like jumping, dashing, attacking, etc.
    /// </summary>
    [Serializable]
    public class EntityAction {
        private bool flagged;
        private BooleanHistoric input;
        private bool overriden;
        private BooleanHistoric overwriteValue;

        public EntityAction() {
            input = new BooleanHistoric();
            overwriteValue = new BooleanHistoric();
        }

        public bool OverwriteValue {
            get => overwriteValue;
            set => overwriteValue.Current = value;
        }

        public bool Current {
            get {
                if (overriden) {
                    return overwriteValue;
                }

                return input.Current;
            }
            set {
                if (overriden) {
                    return;
                }

                input.Current = value;
                if (input.JustActivated) {
                    Set();
                }
            }
        }

        public bool Overriden {
            get => overriden;
            set => overriden = value;
        }

        private void Set() {
            flagged = true;
        }

        public bool Consume() {
            if (!flagged) {
                return false;
            }

            flagged = false;
            return true;
        }

        public bool Peek() {
            return flagged;
        }

        public bool MaybeConsume() {
            return Peek() && Consume();
        }
        public void Clear() {
            Current = false;
        }
    }
}