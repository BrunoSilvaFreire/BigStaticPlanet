namespace Lunari.Tsuki2D.Editor.Movement.Recording {
    public class FlightPlayback {
        private float currentTime;
        private MotorFlight flight;
        private int frame;
        public FlightPlayback(MotorFlight flight) {
            this.flight = flight;
            frame = 0;
        }
        

        public int Frame {
            get => frame;
            set {
                frame = value;
                currentTime = flight.Frames[frame].time;
            }
        }

        public int Step(float deltaTime) {
            currentTime += deltaTime;
            while (flight.Frames[frame].time < currentTime) {
                if (frame < flight.Frames.Count - 1) {
                    frame++;
                } else {
                    // Last frame
                    break;
                }
            }
            return frame;
        }
    }
}