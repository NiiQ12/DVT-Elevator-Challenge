using ElevatorChallenge.Enums;
using ElevatorChallenge.Interfaces;

namespace ElevatorChallenge.Models.Strategy {
    /// <summary>
    /// UninterruptedElevatorStrategy class implements the strategy pattern and provides a method for retrieving an elevator that is either idle 
    /// or that has the least amount of queued stops that will not be interrupted by having to move in the opposite direction in order to pick them up.  
    /// Stopping for people that need to also move in the same direction is permitted.
    /// </summary>
    public class UninterruptedElevatorStrategy : IElevatorStrategy {
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
                .FirstOrDefault(e => {
                    bool correctDirection = e.State == ElevatorState.Idle ||
                                           (e.State == ElevatorState.MovingUp && targetFloor >= e.CurrentFloor) ||
                                           (e.State == ElevatorState.MovingDown && targetFloor <= e.CurrentFloor);

                    return e.State != ElevatorState.Inactive && correctDirection;
                });

            return elevator;
        }
    }
}