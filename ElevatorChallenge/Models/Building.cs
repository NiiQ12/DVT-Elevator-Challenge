namespace ElevatorChallenge.Models {
    /// <summary>
    /// Building class is used to encapsulate a set of elevators
    /// </summary>
    public class Building {
        public string Name;
        public int FloorCount;
        public List<Elevator> Elevators;

        public Building(string name, int floorCount, List<Elevator> elevators) {
            Name = name;
            FloorCount = floorCount;
            Elevators = elevators;
        }

        public void Display() {
            Console.WriteLine($"{Name} Building");
            Console.WriteLine(new string('+', Name.Length + 9));

            foreach (Elevator elevator in Elevators) {
                Console.WriteLine(elevator);
            }

            Console.WriteLine();
        }
    }
}