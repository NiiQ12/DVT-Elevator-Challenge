using ElevatorChallenge.Enums;
using Microsoft.Extensions.Logging;

namespace ElevatorChallenge.Models {
    /// <summary>
    /// Elevator class used to keep track of the state of an elevator as well as the movement, loading-, and unloading of passengers
    /// </summary>
    public class Elevator {
        private readonly ILogger<Elevator> logger;
        private readonly List<ElevatorStop> stops = new List<ElevatorStop>();

        public string? ID { get; set; }
        public int PersonLimit { get; private set; }
        public int PersonCount { get; private set; }
        public int CurrentFloor { get; private set; }
        public ElevatorState State { get; private set; }
        public int StopCount {
            get {
                return stops.Count;
            }
        }

        public Elevator(ILogger<Elevator> logger, ElevatorSettings settings) {
            this.logger = logger;

            PersonLimit = settings.PersonLimit;
            PersonCount = 0;
            CurrentFloor = settings.InitialFloor;
            State = ElevatorState.Idle;
        }

        public void SetID(string id) {
            ID = id;
        }

        public void RequestElevator(int fromFloor, int targetFloor, int personCount) {
            Direction direction = targetFloor > fromFloor ? Direction.Up : Direction.Down;

            int minStopIndex = RequestStop(fromFloor, personCount, 0, direction);
            RequestStop(targetFloor, 0, personCount, direction, minStopIndex);

            if (State == ElevatorState.Idle) {
                Thread thread = new Thread(() => {
                    MoveToNextStop();
                });
                thread.IsBackground = true;
                thread.Start();
            } else {
                logger.LogInformation($"Request queued.");
            }
        }

        private int RequestStop(int floor, int peopleOn, int peopleOff, Direction direction, int minStopIndex = 0) {
            int stopIndex = 0;

            try {
                var stop = stops.Where((o, i) => o.Floor == floor && o.Direction == direction && minStopIndex >= i).First();

                stop.PeopleOn += peopleOn;
                stop.PeopleOff += peopleOff;
            } catch (InvalidOperationException) {
                int index = GetStopIndex(floor, direction);

                stops.Insert(index, new ElevatorStop(floor, peopleOn, peopleOff, direction));
            }

            return stopIndex;
        }

        private int GetStopIndex(int floor, Direction direction) {
            int index = 0;

            if (IsFloorOnRoute(floor, direction)) {
                //Prioritise adding a stop on the current route
                index = stops.FindIndex(stop => stop.Direction == direction && (
                                                (direction == Direction.Up && stop.Floor > floor) ||
                                                (direction == Direction.Down && stop.Floor < floor)));

                if (index == -1) {
                    //If the stop is the last stop in the same direction, add it as the last stop of the current direction
                    index = stops.FindLastIndex(stop => stop.Direction == direction) + 1;
                    if (index == 0) {
                        //If there are no stops in the requested direction, add it as the last stop
                        index = stops.Count;
                    }
                }
            } else {
                //If a stop is not on the current route, add it to the end
                index = stops.Count;
            }

            return index;
        }

        private bool IsFloorOnRoute(int floor, Direction direction) {
            return (floor > CurrentFloor && direction == Direction.Up) ||
                   (floor < CurrentFloor && direction == Direction.Down);
        }

        private void MoveToNextStop() {
            if (stops.Count == 0) {
                State = ElevatorState.Idle;
                logger.LogInformation($"No more stops.");
                return;
            }

            var nextStop = stops[0];

            if (CurrentFloor == nextStop.Floor) {
                //Simulates time it takes to load/unload people;
                Thread.Sleep(1000);

                //Only load people that do not overshoot the limit
                Load((PersonCount + nextStop.PeopleOn > PersonLimit) ? PersonLimit - PersonCount : nextStop.PeopleOn);
                Unload(nextStop.PeopleOff > PersonCount ? PersonCount : nextStop.PeopleOff);

                stops.RemoveAt(0);
            } else if (CurrentFloor < nextStop.Floor) {
                MoveUp();
            } else {
                MoveDown();
            }

            MoveToNextStop();
        }

        private void MoveUp() {
            if (State != ElevatorState.MovingUp)
                State = ElevatorState.MovingUp;

            //Simulates time it takes to travel 1 floor;
            Thread.Sleep(1000);
            CurrentFloor++;
        }

        private void MoveDown() {
            if (State != ElevatorState.MovingDown)
                State = ElevatorState.MovingDown;

            //Simulates time it takes to travel 1 floor;
            Thread.Sleep(1000);
            CurrentFloor--;
        }

        private bool CanLoad(int people) {
            return PersonCount + people <= PersonLimit;
        }

        public void Load(int people) {
            if (CanLoad(people)) {
                PersonCount += people;
                logger.LogInformation($"{ID} loaded {people} people. Total: {PersonCount}/{PersonLimit}");
            } else {
                logger.LogWarning($"Attempted to load {people} people, exceeding limit. Operation aborted.");
                throw new InvalidOperationException("Exceeds elevator limit.");
            }
        }

        public void Unload(int people) {
            if (people <= PersonCount) {
                PersonCount -= people;
                logger.LogInformation($"{ID} unloaded {people} people. Total: {PersonCount}/{PersonLimit}");
            } else {
                logger.LogWarning($"Attempted to unload {people} people, but only {PersonCount} present. Operation aborted.");
                throw new InvalidOperationException("Not enough people to unload.");
            }
        }

        public override string ToString() {
            return $"{(ID != null ? $"Elevator #{ID}\t" : "")}State: {State,10}\tFloor: {CurrentFloor,2}\tCapacity: {PersonCount}/{PersonLimit}";
        }
    }
}