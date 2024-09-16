using System;
using System.Collections.Generic;
using Robot.Common;
#nullable enable

namespace YushkevychAndriiRobotChallange;

public class YushkevychAndriiAlgorythm : IRobotAlgorithm
{
    private int _robotToMoveIndex;
    private IList<Robot.Common.Robot>? _robots;
    private IDictionary<int, ICommand?>? _myRobots;
    private Map? _map;
    private IDictionary<int, EnergyStation>? _occupiedEnergyStations;
    
    public RobotCommand DoStep(IList<Robot.Common.Robot> robots, int robotToMoveIndex, Map map)
    {
        SetupData(robots, robotToMoveIndex, map);
        CalculateStrategy();
        var command = GetRobotCommand();
        var result = command?.Execute();
        
        return result ?? throw new InvalidOperationException();
    }
    private void SetupData(IList<Robot.Common.Robot> robots, int robotToMoveIndex, Map map)
    {
        _myRobots ??= new Dictionary<int, ICommand>()!;
        _occupiedEnergyStations ??= new Dictionary<int, EnergyStation>();
        _map ??= map;

        _robots = robots;
        _robotToMoveIndex = robotToMoveIndex;
        
        foreach (Robot.Common.Robot robot in _robots)
        {
            if (robot.OwnerName == Author && !_myRobots.ContainsKey(_robots.IndexOf(robot)))
            {
                _myRobots.Add(robots.IndexOf(robot), null!);
            }
        }
    }

    public ICommand? GetRobotCommand()
    {
        return _myRobots?[_robotToMoveIndex];
    }

    private void CalculateStrategy()
    {
        if(_myRobots == null || _robots == null)
            throw new NullReferenceException();
        
        switch (_myRobots[_robotToMoveIndex])
        {
            case OccupyStationCommand:
            {
                var command = _myRobots[_robotToMoveIndex] as OccupyStationCommand;

                if (command?.TargetStation.Position == _robots[_robotToMoveIndex].Position)
                {
                    _myRobots[_robotToMoveIndex] = new CollectCommand{TargetStation = command.TargetStation};
                }
                else if(command is not null)
                {
                    _myRobots[_robotToMoveIndex] = new OccupyStationCommand
                    {
                        TargetStation = command.TargetStation,
                        GoToPosition = FindDestination(command.TargetStation,
                            _robots[_robotToMoveIndex], FindDistance(command.TargetStation, _robots[_robotToMoveIndex]))
                    };
                }

                break;
            }
            case CollectCommand:
            {
                var stationPosition = _occupiedEnergyStations?[_robotToMoveIndex].Position;

                if (stationPosition == null || _robots[_robotToMoveIndex].Position != stationPosition)
                {
                    if (stationPosition != null)
                    {
                        _occupiedEnergyStations?.Remove(_robotToMoveIndex);
                    }
                    FindStation(_robotToMoveIndex);
                }
                else if (_robots[_robotToMoveIndex].Energy >= 250 && _myRobots.Count < 100)
                {
                    _myRobots[_robotToMoveIndex] = new BreedCommand { NewRobotEnergy = 100 };
                }

                break;
            }
            case BreedCommand:
            {
                if (_robots[_robotToMoveIndex].Energy < 250)
                {
                    _myRobots[_robotToMoveIndex] = new CollectCommand
                    {
                        TargetStation = _occupiedEnergyStations?[_robotToMoveIndex] ?? null
                    };
                }

                break;
            }
            default:
            {
                FindStation(_robotToMoveIndex);
                break;
            }
        }
    }

    private void FindStation(int myRobot)
    {
        if(_map == null || 
           _robots == null || 
           _myRobots == null || 
           _occupiedEnergyStations == null)
            return;
        
        var distance = Math.Pow(_robots[myRobot].Energy, 2.0);
        EnergyStation? currentStation = null;
            
        foreach (var station in _map.Stations)
        {
            var nextDistance = FindDistance(station, _robots[myRobot]);
                
            if(nextDistance < distance && !_occupiedEnergyStations.Values.Contains(station))
            {
                if (nextDistance < 0.00001)
                {
                    currentStation = null;
                    _myRobots[myRobot] = new CollectCommand{TargetStation = station};
                    break;
                }
                distance = nextDistance;
                currentStation = station;
            }
        }
            
        if(currentStation != null)
        {
            _occupiedEnergyStations.Add(myRobot, currentStation);
            _myRobots[myRobot] = new OccupyStationCommand
            {
                TargetStation = currentStation, 
                GoToPosition = FindDestination(currentStation,
                    _robots[myRobot], distance)
            };
        }

    }

    private Position FindDestination(EnergyStation closestStation, Robot.Common.Robot myRobot, double cost)
    {
        if (cost > myRobot.Energy)
        {
            double fullDistance = Math.Sqrt(cost);
            double currentEnergy = myRobot.Energy;

            for (double currentDistance = fullDistance, k = 1; currentDistance > 0; k++, currentDistance /= k)
            {
                if (Math.Pow(currentDistance, 2.0) * k <= currentEnergy)
                {
                    double ratio = currentDistance / (fullDistance - currentDistance);
                    Position position = new Position(
                        Convert.ToInt32((myRobot.Position.X + ratio * closestStation.Position.X) / (1 + ratio)),
                        Convert.ToInt32((myRobot.Position.Y + ratio * closestStation.Position.Y) / (1 + ratio)));
                    
                    return position;
                    
                }
            }
            
            return myRobot.Position;
        }
        
        return closestStation.Position;
    }

    private double FindDistance(EnergyStation station, Robot.Common.Robot myRobot)
    {
        var x = Math.Pow(station.Position.X - myRobot.Position.X, 2.0);
        var reversedX = Math.Pow(100 - Math.Sqrt(Math.Pow(station.Position.X - myRobot.Position.X, 2.0)), 2.0);
       
        var y = Math.Pow(station.Position.Y - myRobot.Position.Y, 2.0);
        var reversedY = Math.Pow(100 - Math.Sqrt(Math.Pow(station.Position.Y - myRobot.Position.Y, 2.0)), 2.0);
        
        return (x < reversedX ? x : reversedX) + (y < reversedY ? y : reversedY);
    }

    public string Author => "YushkevychAndrii";

    public string Description => "Something";
}