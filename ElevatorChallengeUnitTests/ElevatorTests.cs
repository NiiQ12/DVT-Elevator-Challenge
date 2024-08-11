using ElevatorChallenge.Enums;
using ElevatorChallenge.Models;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace ElevatorChallengeUnitTests {
    [TestFixture]
    public class ElevatorTests {
        private Mock<ILogger<Elevator>> mockLogger;
        private Elevator elevator;

        [SetUp]
        public void Setup() {
            mockLogger = new Mock<ILogger<Elevator>>();
            var settings = new ElevatorSettings {
                PersonLimit = 10,
                InitialFloor = 0
            };
            elevator = new Elevator(mockLogger.Object, settings);
        }

        [Test]
        public void Test_ElevatorInitialState() {
            Assert.Multiple(() => {
                Assert.That(elevator.CurrentFloor, Is.EqualTo(0));
                Assert.That(elevator.State, Is.EqualTo(ElevatorState.Idle));
                Assert.That(elevator.PersonLimit, Is.EqualTo(10));
                Assert.That(elevator.PersonCount, Is.EqualTo(0));
            });
        }

        [Test]
        public void Test_RequestElevator_ValidInput() {
            // Arrange
            int fromFloor = 0;
            int targetFloor = 5;
            int personCount = 3;

            // Act
            elevator.RequestElevator(fromFloor, targetFloor, personCount);

            // Assert
            Assert.That(elevator.StopCount, Is.EqualTo(2));
        }

        [Test]
        public void Test_CanLoad_ValidLoad() {
            // Arrange
            int personCount = 3;

            Assert.Multiple(() => {
                // Act & Assert
                Assert.DoesNotThrow(() => elevator.Load(personCount));
                Assert.That(elevator.PersonCount, Is.EqualTo(personCount));
            });
        }

        [Test]
        public void Test_CanLoad_InvalidLoad() {
            // Arrange
            int personCount = 15;

            Assert.Multiple(() => {
                // Act & Assert
                var ex = Assert.Throws<InvalidOperationException>(() => elevator.Load(personCount));
                Assert.That(ex?.Message, Is.EqualTo("Exceeds elevator limit."));
            });
        }

        [Test]
        public void Test_Unload_ValidUnload() {
            // Arrange
            int personCountLoad = 5;
            int personCountUnload = 3;

            // Arrange
            elevator.Load(personCountLoad);

            Assert.Multiple(() => {
                // Act & Assert
                Assert.DoesNotThrow(() => elevator.Unload(personCountUnload));
                Assert.That(elevator.PersonCount, Is.EqualTo(personCountLoad - personCountUnload));
            });
        }

        [Test]
        public void Test_Unload_InvalidUnload() {
            // Arrange
            int personCount = 3;

            Assert.Multiple(() => {
                // Act & Assert
                var ex = Assert.Throws<InvalidOperationException>(() => elevator.Unload(personCount));
                Assert.That(ex?.Message, Is.EqualTo("Not enough people to unload."));
            });
        }

        [Test]
        public void Test_MoveUp_ChangesFloorAndState() {
            // Arrange
            int targetFloor = 3;

            // Act
            elevator.RequestElevator(0, targetFloor, 1);
            Thread.Sleep(1500); // Simulate time for elevator to pick up 1 person
            Thread.Sleep(3500); // Simulate time for elevator to move 3 floors
            Thread.Sleep(1500); // Simulate time for elevator to drop off 1 person

            // Assert
            Assert.Multiple(() => {
                Assert.That(elevator.CurrentFloor, Is.EqualTo(targetFloor));
                Assert.That(elevator.State, Is.EqualTo(ElevatorState.Idle));
            });
        }

        [Test]
        public void Test_MoveDown_ChangesFloorAndState() {
            // Arrange
            int initialFloor = 3;
            int targetFloor = 0;
            elevator = new Elevator(mockLogger.Object, new ElevatorSettings { PersonLimit = 10, InitialFloor = initialFloor });

            // Act
            elevator.RequestElevator(initialFloor, targetFloor, 1);
            Thread.Sleep(1500); // Simulate time for elevator to pick up 1 person
            Thread.Sleep(3500); // Simulate time for elevator to move 3 floors
            Thread.Sleep(1500); // Simulate time for elevator to drop off 1 person

            // Assert
            Assert.Multiple(() => {
                Assert.That(elevator.CurrentFloor, Is.EqualTo(targetFloor));
                Assert.That(elevator.State, Is.EqualTo(ElevatorState.Idle));
            });
        }
    }
}