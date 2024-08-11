using ElevatorChallenge.Models;

namespace ElevatorChallenge.Interfaces {
    /// <summary>
    /// Interface for implementing the factory pattern
    /// </summary>
    public interface IBuildingFactory {
        public Building CreateBuilding(IServiceProvider sp, string name, int floorCount, int elevatorCount);
    }
}