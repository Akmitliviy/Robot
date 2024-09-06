#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Robot.Common;

namespace YushkevychAndriiRobotChallange;

enum Command
{
    MoveToStation, CreateNewRobot, CollectEnergy, AttackEnemy, None 
}
public class Manager
{
    private Manager(IList<Robot.Common.Robot> robots, int robotToMoveIndex, Map map)
    {
        _myRobots = new Dictionary<int, ICommand>();
        _occupiedEnergyStations = new List<EnergyStation>();

        _robots = robots;
        _robotToMoveIndex = robotToMoveIndex;
        _map = map;
        
        foreach (Robot.Common.Robot robot in _robots)
        {
            if (robot.OwnerName == "YushkevychAndrii")
            {
                _myRobots.Add(robots.IndexOf(robot), null);
            }
        }
    }

    private void SetUpdatedData(IList<Robot.Common.Robot> robots, int robotToMoveIndex)
    {
        _robots = robots;
        _robotToMoveIndex = robotToMoveIndex;

        foreach (Robot.Common.Robot robot in _robots)
        {
            if (robot.OwnerName == "YushkevychAndrii" && !_myRobots.ContainsKey(robots.IndexOf(robot)))
            {
                _myRobots.Add(robots.IndexOf(robot), null);
            }
        }
    }
    
    private static Manager? _instance;

    public static Manager GetManager(IList<Robot.Common.Robot> robots, int robotToMoveIndex, Map map)
    {
        if (_instance == null)
        {
            _instance = new Manager(robots, robotToMoveIndex, map);
        }
        else
        {
            _instance.SetUpdatedData(robots, robotToMoveIndex);
        }

        CalculateStrategy();
        
        return _instance;
    }

    private static int _robotToMoveIndex;
    private static IList<Robot.Common.Robot>? _robots;
    private static IDictionary<int, ICommand>? _myRobots;
    private static Map? _map;
    private static IList<EnergyStation>? _occupiedEnergyStations;

    public ICommand GetRobotCommand()
    {
        return _myRobots[_robotToMoveIndex];
    }

    private static void CalculateStrategy()
    {
        
        foreach (var myRobot in _myRobots.Keys.ToList())
        {

            if (_myRobots[myRobot] == null)
                FindStation(myRobot);
            else
            {
                if (_myRobots[myRobot] is OccupyStationCommand)
                {
                    var command = _myRobots[myRobot] as OccupyStationCommand;

                    if (command.TargetStation.Position == _robots[myRobot].Position)
                    {
                        _myRobots[myRobot] = new CollectCommand();
                    }
                    else
                    {
                        _myRobots[myRobot] = new OccupyStationCommand()
                        {
                            TargetStation = command.TargetStation, 
                            GoToPosition = FindDestination(command.TargetStation,
                                _robots[myRobot], FindDistance(command.TargetStation, _robots[myRobot]))
                        };
                    }
                }
            }


        }
        
        
        
    }

    private static void FindStation(int myRobot)
    {
        
        var distance = 200.0;
        EnergyStation? currentStation = null;
            
        foreach (var station in _map.Stations)
        {
            var nextDistance = FindDistance(station, _robots[myRobot]);
                
            if(nextDistance < distance && !_occupiedEnergyStations.Contains(station))
            {
                distance = nextDistance;
                currentStation = station;
            }
        }
            
        if(currentStation != null)
        {
            _occupiedEnergyStations.Add(currentStation);
            _myRobots[myRobot] = new OccupyStationCommand()
            {
                TargetStation = currentStation, 
                GoToPosition = FindDestination(currentStation,
                    _robots[myRobot], distance)
            };
        }

    }

    private static Position FindDestination(EnergyStation closestStation, Robot.Common.Robot myRobot, double cost)
    {
        if (cost > myRobot.Energy)
        {
            double fullDistance = Math.Sqrt(cost);
            double currentEnergy = myRobot.Energy;

            for (double currentDistance = fullDistance; currentDistance > 0; --currentDistance)
            {
                if (Math.Pow(currentDistance, 2.0) + Math.Pow(fullDistance - currentDistance, 2.0) <= currentEnergy)
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

    private static double FindDistance(EnergyStation station, Robot.Common.Robot myRobot)
    {
        return Math.Abs(Math.Pow(station.Position.X - myRobot.Position.X, 2.0) + Math.Pow(station.Position.Y - myRobot.Position.Y, 2.0));
    }
    
}