using NUnit.Framework;
using System.Collections.Generic;
using YushkevychAndriiRobotChallange;
using Robot.Common;
using System.Reflection;
using YushkevychAndriiRobotChallange.Commands;

namespace YushkevychAndriiRobotChallange.Tests
{
    [TestFixture]
    public class GetToOtherSideTest
    
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
                    new (){Position = new Position(99, 0)},
                    new (){Position = new Position(0, 99)}
                }
            };

            _robots = new List<Robot.Common.Robot>
            {
                new()
                {
                    Position = new Position(0, 0),
                    Energy = 150,
                    OwnerName = "YushkevychAndrii"
                },
                new()
                {
                    Position = new Position(3, 0),
                    Energy = 150,
                    OwnerName = "YushkevychAndrii"
                }
            };

            _algorithm = new YushkevychAndriiAlgorithm();
        }

        [Test]
        public void RobotGoToOtherSide()
        {
            _algorithm.DoStep(_robots, 0, _map);
            
            var command = _algorithm.GetRobotCommand();

            Assert.IsInstanceOf<OccupyStationCommand>(command);

            var occupyStationCommand = command as OccupyStationCommand;
            _robots[0].Position = occupyStationCommand?.GoToPosition;
            
            _algorithm.DoStep(_robots, 0, _map);
            Assert.That(_robots[0].Position, Is.EqualTo(_map.Stations[0].Position));
            
            _algorithm.DoStep(_robots, 1, _map);
            
            command = _algorithm.GetRobotCommand();

            Assert.IsInstanceOf<OccupyStationCommand>(command);

            occupyStationCommand = command as OccupyStationCommand;
            _robots[1].Position = occupyStationCommand?.GoToPosition;
            
            _algorithm.DoStep(_robots, 1, _map);
            Assert.That(_robots[1].Position, Is.EqualTo(_map.Stations[1].Position));
        }
    }
}
