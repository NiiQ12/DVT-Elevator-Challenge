using ElevatorChallenge.Enums;
using ElevatorChallenge.Interfaces;
using ElevatorChallenge.Models;
using ElevatorChallenge.Models.Factory;
using ElevatorChallenge.Models.Strategy;
using log4net;
using log4net.Config;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Reflection;

namespace ElevatorChallenge {
    public partial class Program {
        static void Main(string[] args) {
            var serviceProvider = ConfigureServices();
            var buildingSettings = serviceProvider.GetRequiredService<IOptions<BuildingSettings>>().Value;

            BuildingFactory buildingFactory = new BuildingFactory();
            Building building = buildingFactory.CreateBuilding(serviceProvider, buildingSettings.Name, buildingSettings.FloorCount, buildingSettings.ElevatorCount);
            ElevatorController controller = new ElevatorController(building);

            DisplayStates(controller);

            int currentFloor;
            int targetFloor;
            int people;
            string menuOption;
            IElevatorStrategy? elevatorStrategy = null;

            switch (buildingSettings.ElevatorStrategy) {
                case ElevatorStrategy.GetNearestElevator:
                    elevatorStrategy = new NearestElevatorStrategy();
                    break;
                case ElevatorStrategy.GetAvailableElevator:
                    elevatorStrategy = new AvailableElevatorStrategy();
                    break;
                case ElevatorStrategy.GetUninterruptedElevator:
                    elevatorStrategy = new UninterruptedElevatorStrategy();
                    break;
            }

            while (true) {
                Start:
                do {
                    Console.Write($"What floor (0-{controller.Building.FloorCount - 1}) requires pick-up? [B. Back]\t");
                    menuOption = Console.ReadLine() ?? "";

                    if (menuOption?.ToUpper() == "B") goto Start;
                } while (!int.TryParse(menuOption, out currentFloor) || currentFloor < 0 || currentFloor > 20);

                do {
                    Console.Write($"What floor (0-{controller.Building.FloorCount - 1}) should the elevator go to? [B. Back]\t");
                    menuOption = Console.ReadLine() ?? "";

                    if (menuOption?.ToUpper() == "B") goto Start;
                } while (!int.TryParse(menuOption, out targetFloor) || targetFloor < 0 || targetFloor > 20);

                do {
                    Console.Write($"How many people (1-{controller.Building.Elevators[0].PersonLimit}) need the elevator? [B. Back]\t");
                    menuOption = Console.ReadLine() ?? "";

                    if (menuOption?.ToUpper() == "B") goto Start;
                } while (!int.TryParse(menuOption, out people) || people < 1 || people > 10);

                controller.RequestElevator(elevatorStrategy, currentFloor, targetFloor, people);
                Console.WriteLine();
            }
        }

        private static IServiceProvider ConfigureServices() {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddXmlFile("log4net.config", optional: true, reloadOnChange: true)
                .Build();

            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));

            var serviceProvider = new ServiceCollection()
                .AddLogging(builder => {
                    builder.ClearProviders(); // Remove other log providers
                    builder.AddProvider(new Log4NetLoggerProvider()); // Add log4net provider
                })
                .Configure<BuildingSettings>(config.GetSection("BuildingSettings"))
                .Configure<ElevatorSettings>(config.GetSection("ElevatorSettings"))
                .AddTransient<Elevator>(sp => {
                    var settings = sp.GetRequiredService<IOptions<ElevatorSettings>>().Value;
                    return new Elevator(sp.GetRequiredService<ILogger<Elevator>>(), settings);
                })
                .BuildServiceProvider();

            return serviceProvider;
        }

        private static void DisplayStates(ElevatorController controller) {
            // Start a thread to update the display of the 2D array
            Thread displayThread = new Thread(() => {
                while (true) {
                    (int left, int top) = Console.GetCursorPosition();
                    
                    Console.SetCursorPosition(0, 0);
                    controller.DisplayElevatorStatuses();

                    if (top != 0) Console.SetCursorPosition(left, top);
                    Thread.Sleep(1000);
                }
            });

            displayThread.Start();
            Thread.Sleep(1000);
        }
    }
}