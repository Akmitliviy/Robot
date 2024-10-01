using Robot.Common;
using YushkevychAndriiRobotChallenge;
using YushkevychAndriiRobotChallenge.Commands;

namespace RobotTests
{
    [TestFixture]
    public class MovementTest
    
    {
        private IList<Robot.Common.Robot> _robots;
        private Map _map;
        private YushkevychAndriiAlgorithm _algorithm;
        
        [SetUp]
        public void SetUp()
        {

            _map = new Map
            {
                Stations = new List<EnergyStation>
                {
                    new(){Position = new Position(35, 35)},
                    new(){Position = new Position(50, 50)},
                    new(){Position = new Position(5, 50)},
                    new(){Position = new Position(5, 65)},
                    new (){Position = new Position(99, 0)},
                    new (){Position = new Position(0, 95)}
                }
            };

            _robots = new List<Robot.Common.Robot>
            {
                new()
                {
                    Position = new Position(40, 40),
                    Energy = 150,
                    OwnerName = "YushkevychAndrii"
                },
                new()
                {
                    Position = new Position(60, 60),
                    Energy = 150,
                    OwnerName = "YushkevychAndrii"
                },
                new()
                {
                    Position = new Position(0, 45),
                    Energy = 150,
                    OwnerName = "YushkevychAndrii"
                },
                new()
                {
                    Position = new Position(0, 55),
                    Energy = 150,
                    OwnerName = "YushkevychAndrii"
                },
                new()
                {
                    Position = new Position(0, 0),
                    Energy = 150,
                    OwnerName = "YushkevychAndrii"
                },
                new()
                {
                    Position = new Position(3, 3),
                    Energy = 70,
                    OwnerName = "YushkevychAndrii"
                }
            };

            _algorithm = new YushkevychAndriiAlgorithm();
        }

        [Test]
        public void OneRobotGetToStation()
        {
            _algorithm.DoStep(_robots, 0, _map);
            var command = _algorithm.GetRobotCommand();

            Assert.IsInstanceOf<OccupyStationCommand>(command);

            var occupyStationCommand = command as OccupyStationCommand;
            _robots[0].Position = occupyStationCommand?.GoToPosition;
            
            _algorithm.DoStep(_robots, 0, _map);
            command = _algorithm.GetRobotCommand();

            Assert.IsInstanceOf<CollectCommand>(command);
            Assert.That(_robots[0].Position, Is.EqualTo(_map.Stations[0].Position));
        }

        [Test]
        public void OneRobotGetToStationLongDistance()
        {
            _algorithm.DoStep(_robots, 1, _map);
            var command = _algorithm.GetRobotCommand();
            Assert.IsInstanceOf<OccupyStationCommand>(command);
            var newPosition = (command as OccupyStationCommand)?.GoToPosition;
            
            _robots[1].Energy -= Convert.ToInt32(
                Math.Pow(newPosition.X - _robots[1].Position.X, 2) + 
                Math.Pow(newPosition.Y - _robots[1].Position.Y, 2));
            _robots[1].Position = newPosition;
            
            _algorithm.DoStep(_robots, 1, _map);
            command = _algorithm.GetRobotCommand();
            Assert.IsInstanceOf<OccupyStationCommand>(command);
            newPosition = (command as OccupyStationCommand)?.GoToPosition;
            
            _robots[1].Energy -= Convert.ToInt32(
                Math.Pow(newPosition.X - _robots[1].Position.X, 2) + 
                Math.Pow(newPosition.Y - _robots[1].Position.Y, 2));
            _robots[1].Position = newPosition;
            
            _algorithm.DoStep(_robots, 1, _map);
            Assert.IsInstanceOf<CollectCommand>(_algorithm.GetRobotCommand());
            
            Assert.That(_robots[1].Position, Is.EqualTo(_map.Stations[1].Position));
        }

        [Test]
        public void TwoRobotsSameClosestStation()
        {
            _algorithm.DoStep(_robots, 2, _map);
            var command = _algorithm.GetRobotCommand();
            Assert.IsInstanceOf<OccupyStationCommand>(command);
            var newPosition = (command as OccupyStationCommand)?.GoToPosition;
            _robots[2].Position = newPosition;
            Assert.That(_robots[2].Position, Is.EqualTo(_map.Stations[2].Position));
            
            
            _algorithm.DoStep(_robots, 3, _map);
            command = _algorithm.GetRobotCommand();
            Assert.IsInstanceOf<OccupyStationCommand>(command);
            newPosition = (command as OccupyStationCommand)?.GoToPosition;
            _robots[3].Position = newPosition;
            Assert.That(_robots[3].Position, Is.EqualTo(_map.Stations[3].Position));
            
        }
        [Test]
        public void RobotGoToOtherSide()
        {
            _algorithm.DoStep(_robots, 4, _map);
            
            var command = _algorithm.GetRobotCommand();

            Assert.IsInstanceOf<OccupyStationCommand>(command);

            var occupyStationCommand = command as OccupyStationCommand;
            _robots[4].Position = occupyStationCommand?.GoToPosition;
            
            _algorithm.DoStep(_robots, 4, _map);
            Assert.That(_robots[4].Position, Is.EqualTo(_map.Stations[4].Position));
            
            _algorithm.DoStep(_robots, 5, _map);
            
            command = _algorithm.GetRobotCommand();

            Assert.IsInstanceOf<OccupyStationCommand>(command);

            occupyStationCommand = command as OccupyStationCommand;
            _robots[5].Position = occupyStationCommand?.GoToPosition;
            
            _algorithm.DoStep(_robots, 5, _map);
            command = _algorithm.GetRobotCommand();

            Assert.IsInstanceOf<OccupyStationCommand>(command);

            occupyStationCommand = command as OccupyStationCommand;
            _robots[5].Position = occupyStationCommand?.GoToPosition;
            
            _algorithm.DoStep(_robots, 5, _map);
            Assert.That(_robots[5].Position, Is.EqualTo(_map.Stations[5].Position));
        }
    }
}
