namespace Lunari.Tsuki2D.Runtime.Input.TopDown {

    public class TopDownInput : EntityInput<TopDownInputSource> {
        public float vertical;
        public float horizontal;

        protected override void Clear() {
            vertical = 0;
            horizontal = 0;
        }
        protected override void ReadInput(TopDownInputSource src) {
            vertical = src.GetVertical();
            horizontal = src.GetHorizontal();
        }
    }
}