using System;
using Lunari.Tsuki.Misc;
using Lunari.Tsuki2D.Runtime.Input;
namespace Lunari.Tsuki2D.Platforming.Input {
    [Serializable]
    public class SerializablePlatformerInput : SerializableInterface<IPlatformerInput> {

    }
    public abstract class PlatformerInputSource : InputSource {
        public abstract float GetHorizontal();
        public abstract bool GetJump();
    }
}