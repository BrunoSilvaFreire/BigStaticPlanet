using Lunari.Tsuki2D.Runtime.Input;
namespace Lunari.Tsuki2D.Platforming.Input {
    public interface IPlatformerInput {

        float Horizontal {
            get;
            set;
        }

        EntityAction Jump {
            get;
        }
    }
}