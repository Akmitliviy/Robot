using Robot.Common;
using YushkevychAndriiRobotChallange;
using YushkevychAndriiRobotChallange.Commands;

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
                    new(){Position = new Position(5, 5)}
                }
            };

            _robots = new List<Robot.Common.Robot>
            {
                new()
                {
                    Position = new Position(0, 0),
                    Energy = 150,
                    OwnerName = "YushkevychAndrii"
                }
            };

            _algorithm = new YushkevychAndriiAlgorithm();
        }

        [Test]
        public void OneRobotGetToStation()
        {
            var newPosition = new Position(4, 5);
            var oldPosition = new Position(1, 2);
            
            var newX = (2 * newPosition.X - oldPosition.X) % 100;
            var newY = (2 * newPosition.Y - oldPosition.Y) % 100;
            
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
    }
}
