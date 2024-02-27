namespace Lunari.Tsuki2D.Runtime.Input {
    public class DiscreteInputState {
        private bool set;

        public void Set() {
            set = true;
        }

        public bool Consume() {
            if (!set) {
                return false;
            }

            set = false;
            return true;
        }

        public bool Peek() {
            return set;
        }

        public bool MaybeConsume() {
            return Peek() && Consume();
        }
    }
}