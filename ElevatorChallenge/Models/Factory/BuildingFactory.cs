using ElevatorChallenge.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace ElevatorChallenge.Models.Factory {
    /// <summary>
    /// BuildingFactory class used to create buildings with applicable numbers of elevators
    /// </summary>
    public class BuildingFactory : IBuildingFactory {
        public Building CreateBuilding(IServiceProvider sp, string name, int floorCount, int elevatorCount) {
            var elevators = Enumerable.Range(0, elevatorCount)
                                      .Select(i => {
                                          var elevator = sp.GetRequiredService<Elevator>();
                                          elevator.SetID($"ELE{(i + 1).ToString().PadLeft(3, '0')}");
                                          return elevator;
                                      })
                                      .ToList();

            return new Building(name, floorCount, elevators);
        }
    }
}