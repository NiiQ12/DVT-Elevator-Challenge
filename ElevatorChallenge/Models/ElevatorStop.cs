using ElevatorChallenge.Enums;

namespace ElevatorChallenge.Models {
    /// <summary>
    /// ElevatorStop class is used to allow for queueing of stops
    /// </summary>
    public class ElevatorStop {
        public int Floor { get; set; }
        public int PeopleOn { get; set; }
        public int PeopleOff { get; set; }
        public Direction Direction { get; set; }

        public ElevatorStop(int floor, int peopleOn, int peopleOff, Direction direction) {
            Floor = floor;
            PeopleOn = peopleOn;
            PeopleOff = peopleOff;
            Direction = direction;
        }
    }
}