using ElevatorChallenge.Interfaces;
using ElevatorChallenge.Models;

namespace ElevatorChallenge {
    /// <summary>
    /// ElevatorController class is the mediator between the client application and the elevators
    /// </summary>
    public class ElevatorController {
        public Building Building;

        public ElevatorController(Building building) {
            Building = building;
        }

        public void RequestElevator(IElevatorStrategy? elevatorStrategy, int fromFloor, int targetFloor, int personCount) {
            Elevator? elevator;
            try {
                if (elevatorStrategy != null) {
                    elevator = elevatorStrategy.GetElevator(Building, fromFloor, personCount);
                } else {
                    elevator = Building.Elevators[0];
                }

                if (elevator == null) throw new InvalidOperationException();
            } catch (InvalidOperationException) {
                Console.WriteLine("\nNo available elevator.");
                return;
            } catch (Exception) {
                Console.WriteLine("\nUnable to allocate an elevator.");
                return;
            }

            elevator.RequestElevator(fromFloor, targetFloor, personCount);
        }

        public void DisplayElevatorStatuses() {
            Building.Display();
        }
    }
}