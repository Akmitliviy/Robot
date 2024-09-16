using NUnit.Framework;
using System.Collections.Generic;
using YushkevychAndriiRobotChallange;
using Robot.Common;
using System.Reflection;

namespace YushkevychAndriiRobotChallange.Tests
{
    [TestFixture]
    public class BreedTest
    
    {
        private IList<Robot.Common.Robot> _robots;
        private Map _map;
        private YushkevychAndriiAlgorythm _algorithm;
        
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
                    Energy = 300,
                    OwnerName = "YushkevychAndrii"
                }
            };

            _algorithm = new YushkevychAndriiAlgorythm();
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
            
            _algorithm.DoStep(_robots, 0, _map);
            command = _algorithm.GetRobotCommand();

            Assert.IsInstanceOf<BreedCommand>(command);
        }
    }
}