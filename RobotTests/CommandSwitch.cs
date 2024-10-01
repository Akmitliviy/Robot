using Robot.Common;
using YushkevychAndriiRobotChallenge;
using YushkevychAndriiRobotChallenge.Commands;

namespace RobotTests;

[TestFixture]
public class CommandSwitch
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
                new(){Position = new Position(5, 5)},
                new(){Position = new Position(5, 20), Energy = 30},
                new(){Position = new Position(5, 40)},
                new(){Position = new Position(5, 60)},
                new(){Position = new Position(5, 80)}
            }
        };

        _robots = new List<Robot.Common.Robot>
        {
            new()
            {
                Position = new Position(5, 4),
                Energy = 100,
                OwnerName = "YushkevychAndrii"
            },
            new()
            {
                Position = new Position(5, 20),
                Energy = 190,
                OwnerName = "YushkevychAndrii"
            },
            new()
            {
                Position = new Position(5, 40),
                Energy = 190,
                OwnerName = "YushkevychAndrii"
            },
            new()
            {
                Position = new Position(5, 60),
                Energy = 250,
                OwnerName = "YushkevychAndrii"
            },
            new()
            {
                Position = new Position(5, 80),
                Energy = 250,
                OwnerName = "YushkevychAndrii"
            }
        };

        _algorithm = new YushkevychAndriiAlgorithm();
    }

    [Test]
    public void OccupyToCollect()
    {
        
        _algorithm.DoStep(_robots, 0, _map);
        var command = _algorithm.GetRobotCommand();

        Assert.IsInstanceOf<OccupyStationCommand>(command);
        _robots[0].Position = (command as OccupyStationCommand)?.GoToPosition;
        
        _algorithm.DoStep(_robots, 0, _map);

        Assert.IsInstanceOf<CollectCommand>(_algorithm.GetRobotCommand());
    }

    [Test]
    public void CollectToBreed()
    {
        _algorithm.DoStep(_robots, 1, _map);

        Assert.IsInstanceOf<CollectCommand>(_algorithm.GetRobotCommand());
        _robots[1].Energy += _map.Stations[1].Energy;
        
        _algorithm.DoStep(_robots, 1, _map);

        Assert.IsInstanceOf<BreedCommand>(_algorithm.GetRobotCommand());
    }

    [Test]
    public void CollectToOccupy()
    {
        _algorithm.DoStep(_robots, 2, _map);

        Assert.IsInstanceOf<CollectCommand>(_algorithm.GetRobotCommand());
        _robots[2].Position = new Position(5, 24);
        
        _algorithm.DoStep(_robots, 2, _map);

        Assert.IsInstanceOf<OccupyStationCommand>(_algorithm.GetRobotCommand());
    }

    [Test]
    public void BreedToCollect()
    {
        _algorithm.DoStep(_robots, 3, _map);
        Assert.IsInstanceOf<BreedCommand>(_algorithm.GetRobotCommand());
        
        _robots[3].Energy -= 100;
        
        _algorithm.DoStep(_robots, 3, _map);
        Assert.IsInstanceOf<CollectCommand>(_algorithm.GetRobotCommand());
    }

    [Test]
    public void BreedToOccupy()
    {
        _algorithm.DoStep(_robots, 4, _map);
        Assert.IsInstanceOf<BreedCommand>(_algorithm.GetRobotCommand());
        
        _robots[4].Position = new Position(5, 78);
        _robots[4].Energy -= 100;
        
        _algorithm.DoStep(_robots, 4, _map);
        Assert.IsInstanceOf<OccupyStationCommand>(_algorithm.GetRobotCommand());
    }
}