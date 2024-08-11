using ElevatorChallenge.Enums;
using ElevatorChallenge.Interfaces;

namespace ElevatorChallenge.Models.Strategy {
    /// <summary>
    /// NearestElevatorStrategy class implements the strategy pattern and provides a method for retrieving the nearest elevator at the time
    /// of the request
    /// </summary>
    public class NearestElevatorStrategy : IElevatorStrategy {
        Elevator? IElevatorStrategy.GetElevator(Building building, int targetFloor, int personCount) {
            var elevator = building.Elevators
                .OrderBy(e => Math.Abs(targetFloor - e.CurrentFloor))
                .FirstOrDefault(e => e.State != ElevatorState.Inactive);

            return elevator;
        }
    }
}