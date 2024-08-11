namespace ElevatorChallenge.Enums {
    /// <summary>
    /// ElevatorStrategy enum is used to determine the strategy for fetching an elevator
    /// </summary>
    public enum ElevatorStrategy {
        GetNearestElevator,
        GetAvailableElevator,
        GetUninterruptedElevator
    }
}