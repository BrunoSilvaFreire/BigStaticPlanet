using UnityEngine;
namespace Lunari.Tsuki2D.Runtime.Movement.Updaters {
    public abstract class SupportStateUpdater : MonoBehaviour {
        public abstract void Test(Motor motor, ref SupportState state);
    }
}