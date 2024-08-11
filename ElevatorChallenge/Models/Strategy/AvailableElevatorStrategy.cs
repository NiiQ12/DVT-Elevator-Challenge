using ElevatorChallenge.Enums;
using ElevatorChallenge.Interfaces;

namespace ElevatorChallenge.Models.Strategy {
    /// <summary>
    /// AvailableElevatorStrategy class implements the strategy pattern and provides a method for retrieving an elevator that is either idle 
    /// or that has the least amount of queued stops
    /// </summary>
    public class AvailableElevatorStrategy : IElevatorStrategy {
        Elevator? IElevatorStrategy.GetElevator(Building building, int targetFloor, int personCount) {
            var elevator = building.Elevators
                .OrderBy(e => {
                    return e.State switch {
                        ElevatorState.Inactive => 2,
                        ElevatorState.Idle => 0,
                        ElevatorState.MovingUp => 1,
                        ElevatorState.MovingDown => 1,
                        _ => 2,
                    };
                })
                .ThenBy(e => e.StopCount)
                .FirstOrDefault(e => e.State != ElevatorState.Inactive);

            return elevator;
        }
    }
}