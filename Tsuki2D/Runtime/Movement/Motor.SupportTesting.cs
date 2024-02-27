using Lunari.Tsuki2D.Runtime.Movement.Updaters;
namespace Lunari.Tsuki2D.Runtime.Movement {
    public partial class Motor {
        public SupportStateUpdater updater;

        private void ConsumeAndUpdateSupportState() {
            if (updater != null) {
                updater.Test(this, ref supportState);
            }
        }
    }
}