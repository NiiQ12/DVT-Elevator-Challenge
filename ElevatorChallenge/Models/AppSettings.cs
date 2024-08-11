using ElevatorChallenge.Enums;

namespace ElevatorChallenge.Models {
    /// <summary>
    /// BuildingSettings class is used to retrieve and utilise settings associated with the building
    /// </summary>
    public class BuildingSettings {
        public string Name { get; set; }
        public int FloorCount { get; set; }
        public int ElevatorCount { get; set; }
        public ElevatorStrategy ElevatorStrategy { get; set; }
    }

    /// <summary>
    /// ElevatorSettings class is used to retrieve and utilise settings associated with elevators
    /// </summary>
    public class ElevatorSettings {
        public int InitialFloor { get; set; }
        public int PersonLimit { get; set; }
    }
}