using ElevatorChallenge.Models;

namespace ElevatorChallenge.Interfaces {
    /// <summary>
    /// IGetElevatorStrategy interface used as a template to apply the Strategy pattern
    /// </summary>
    public interface IElevatorStrategy {
        Elevator? GetElevator(Building building, int targetFloor, int personCount);
    }
}