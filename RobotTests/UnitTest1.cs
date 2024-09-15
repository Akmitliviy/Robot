using NUnit.Framework;
using System.Collections.Generic;
using YushkevychAndriiRobotChallange;
using Robot.Common;
using System.Reflection;

namespace YushkevychAndriiRobotChallange.Tests
{
    [TestFixture]
    public class ManagerTests
    {
        private IList<Robot.Common.Robot> _robots;
        private Dictionary<int, ICommand> _myRobots;
        private Map _map;
        private EnergyStation _station1, _station2;
        
        [SetUp]
        public void SetUp()
        {
            _station1 = new EnergyStation { Position = new Position(0, 0) };
            _station2 = new EnergyStation { Position = new Position(10, 10) };

            _map = new Map { Stations = new List<EnergyStation> { _station1, _station2 } };

            _robots = new List<Robot.Common.Robot>
            {
                new Robot.Common.Robot { Position = new Position(1, 1), Energy = 100, OwnerName = "YushkevychAndrii" },
                new Robot.Common.Robot { Position = new Position(2, 2), Energy = 200, OwnerName = "OtherOwner" }
            };
        }

        [Test]
        public void GetManager_CreatesNewInstance_WhenCalledFirstTime()
        {
            var manager = Manager.GetManager(_robots, 0, _map);

            Assert.NotNull(manager);
        }

        [Test]
        public void GetManager_UpdatesData_WhenCalledSubsequentTimes()
        {
            var manager = Manager.GetManager(_robots, 0, _map);
            var newRobots = new List<Robot.Common.Robot>(_robots)
            {
                new Robot.Common.Robot { Position = new Position(3, 3), Energy = 300, OwnerName = "YushkevychAndrii" }
            };

            var updatedManager = Manager.GetManager(newRobots, 2, _map);

            Assert.AreEqual(manager, updatedManager);
        }

        [Test]
        public void GetRobotCommand_ReturnsNullCommand_WhenNoCommandAssigned()
        {
            var manager = Manager.GetManager(_robots, 0, _map);

            var command = manager.GetRobotCommand();

            Assert.IsNull(command);
        }

        [Test]
        public void FindStation_AssignsOccupyStationCommand_WhenStationIsFree()
        {
            var manager = Manager.GetManager(_robots, 0, _map);

            MethodInfo findStationMethod = typeof(Manager).GetMethod("FindStation", BindingFlags.NonPublic | BindingFlags.Static);
            findStationMethod.Invoke(manager, new object[] { 0 });

            var command = manager.GetRobotCommand();
            Assert.IsInstanceOf<OccupyStationCommand>(command);
        }

        [Test]
        public void FindStation_AssignsCollectCommand_WhenRobotIsAtStation()
        {
            _robots[0].Position = _station1.Position;
            var manager = Manager.GetManager(_robots, 0, _map);

            MethodInfo findStationMethod = typeof(Manager).GetMethod("FindStation", BindingFlags.NonPublic | BindingFlags.Static);
            findStationMethod.Invoke(manager, new object[] { 0 });

            var command = manager.GetRobotCommand();
            Assert.IsInstanceOf<CollectCommand>(command);
        }

        [Test]
        public void CalculateStrategy_CollectCommandToBreedCommand_WhenEnergyIsSufficient()
        {
            _robots[0].Energy = 300;
            var manager = Manager.GetManager(_robots, 0, _map);

            MethodInfo calculateStrategyMethod = typeof(Manager).GetMethod("CalculateStrategy", BindingFlags.NonPublic | BindingFlags.Static);
            calculateStrategyMethod.Invoke(manager, null);

            var command = manager.GetRobotCommand();
            Assert.IsInstanceOf<BreedCommand>(command);
        }

        [Test]
        public void CalculateStrategy_BreedCommandToCollectCommand_WhenEnergyIsInsufficient()
        {
            _robots[0].Energy = 150;
            var manager = Manager.GetManager(_robots, 0, _map);

            MethodInfo calculateStrategyMethod = typeof(Manager).GetMethod("CalculateStrategy", BindingFlags.NonPublic | BindingFlags.Static);
            _myRobots[0] = new BreedCommand();

            calculateStrategyMethod.Invoke(manager, null);

            var command = manager.GetRobotCommand();
            Assert.IsInstanceOf<CollectCommand>(command);
        }

        [Test]
        public void FindDestination_ReturnsCorrectPosition_WhenRobotHasSufficientEnergy()
        {
            var manager = Manager.GetManager(_robots, 0, _map);

            MethodInfo findDestinationMethod = typeof(Manager).GetMethod("FindDestination", BindingFlags.NonPublic | BindingFlags.Static);
            var destination = (Position)findDestinationMethod.Invoke(manager, new object[] { _station1, _robots[0], 10.0 });

            Assert.AreEqual(_station1.Position, destination);
        }

        [Test]
        public void FindDestination_ReturnsCurrentPosition_WhenRobotHasInsufficientEnergy()
        {
            _robots[0].Energy = 5;
            var manager = Manager.GetManager(_robots, 0, _map);

            MethodInfo findDestinationMethod = typeof(Manager).GetMethod("FindDestination", BindingFlags.NonPublic | BindingFlags.Static);
            var destination = (Position)findDestinationMethod.Invoke(manager, new object[] { _station1, _robots[0], 100.0 });

            Assert.AreEqual(_robots[0].Position, destination);
        }

        [Test]
        public void FindDistance_ReturnsCorrectDistance_BetweenRobotAndStation()
        {
            var manager = Manager.GetManager(_robots, 0, _map);

            MethodInfo findDistanceMethod = typeof(Manager).GetMethod("FindDistance", BindingFlags.NonPublic | BindingFlags.Static);
            var distance = (double)findDistanceMethod.Invoke(manager, new object[] { _station1, _robots[0] });

            Assert.AreEqual(2.0, distance, 0.001);
        }
    }
}
