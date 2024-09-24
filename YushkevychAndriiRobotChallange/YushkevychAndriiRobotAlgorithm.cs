using System;
using System.Collections.Generic;
using Robot.Common;
using YushkevychAndriiRobotChallange.Commands;
using YushkevychAndriiRobotChallange.Exceptions;

#nullable enable

namespace YushkevychAndriiRobotChallange;

public class YushkevychAndriiAlgorithm : IRobotAlgorithm
{
    internal int RobotToMoveIndex;
    internal IList<Robot.Common.Robot>? Robots;
    internal IDictionary<int, Command?>? MyRobots;
    internal Map? Map;
    internal IDictionary<int, EnergyStation>? OccupiedEnergyStations;

    public RobotCommand DoStep(IList<Robot.Common.Robot> robots, int robotToMoveIndex, Map map)
    {
        SetupData(robots, robotToMoveIndex, map);
        //CalculateStrategy();
        var command = GetRobotCommand();
        if (command is null)
        {
            var station = FindStation(robotToMoveIndex);
            if (station is null)
                throw new NoAvailableStationException("Could not find station to move to");
            
            if(station.Position == Robots?[RobotToMoveIndex].Position)
                TransitionTo(new CollectCommand{TargetStation = station});
            else
                TransitionTo(new OccupyStationCommand
                {
                    TargetStation = station,
                    GoToPosition = Movement.FindDestination(station,
                        Robots?[RobotToMoveIndex], 
                        Movement.FindDistance(station, Robots?[RobotToMoveIndex]))
                });
            
            command = GetRobotCommand();
        }
        command?.Process();
        var result = command?.Execute();

        return result ?? throw new InvalidOperationException();
    }

    private void SetupData(IList<Robot.Common.Robot> robots, int robotToMoveIndex, Map map)
    {
        MyRobots ??= new Dictionary<int, Command>()!;
        OccupiedEnergyStations ??= new Dictionary<int, EnergyStation>();
        Map ??= map;

        Robots = robots;
        RobotToMoveIndex = robotToMoveIndex;

        foreach (Robot.Common.Robot robot in Robots)
        {
            if (robot.OwnerName == Author && !MyRobots.ContainsKey(Robots.IndexOf(robot)))
            {
                MyRobots.Add(robots.IndexOf(robot), null);
            }
        }
    }

    public void TransitionTo(Command command)
    {
        if (MyRobots == null)
            throw new RobotsNullReferenceException("_myRobots is null");

        MyRobots[RobotToMoveIndex] = command;
        MyRobots[RobotToMoveIndex]!.SetAlgorithm(this);
    }

    public Command? GetRobotCommand()
    {
        return MyRobots?[RobotToMoveIndex];
    }

    private void CalculateStrategy()
    {
        if (MyRobots is null)
            throw new RobotsNullReferenceException("_myRobots is null");

        if (Robots is null)
            throw new RobotsNullReferenceException("_robots is null");

        switch (MyRobots[RobotToMoveIndex])
        {
            case OccupyStationCommand:
            {
                var command = MyRobots[RobotToMoveIndex] as OccupyStationCommand;

                if (command?.TargetStation.Position == Robots[RobotToMoveIndex].Position)
                {
                    MyRobots[RobotToMoveIndex] = new CollectCommand { TargetStation = command.TargetStation };
                }
                else if (command is not null)
                {
                    MyRobots[RobotToMoveIndex] = new OccupyStationCommand
                    {
                        TargetStation = command.TargetStation,
                        GoToPosition = Movement.FindDestination(command.TargetStation,
                            Robots[RobotToMoveIndex],
                            Movement.FindDistance(command.TargetStation, Robots[RobotToMoveIndex]))
                    };
                }

                break;
            }
            case CollectCommand:
            {
                var stationPosition = OccupiedEnergyStations?[RobotToMoveIndex].Position;

                if (stationPosition == null || Robots[RobotToMoveIndex].Position != stationPosition)
                {
                    if (stationPosition != null)
                    {
                        OccupiedEnergyStations?.Remove(RobotToMoveIndex);
                    }

                    FindStation(RobotToMoveIndex);
                }
                else if (Robots[RobotToMoveIndex].Energy >= 250 && MyRobots.Count < 100)
                {
                    MyRobots[RobotToMoveIndex] = new BreedCommand { NewRobotEnergy = 100 };
                }

                break;
            }
            case BreedCommand:
            {
                if (Robots[RobotToMoveIndex].Energy < 250)
                {
                    MyRobots[RobotToMoveIndex] = new CollectCommand
                    {
                        TargetStation = OccupiedEnergyStations?[RobotToMoveIndex] ?? null
                    };
                }

                break;
            }
            default:
            {
                FindStation(RobotToMoveIndex);
                break;
            }
        }
    }

    internal EnergyStation? FindStation(int myRobot)
    {
        if (Map == null)
            throw new RobotsNullReferenceException("_map is null");

        if (Robots == null)
            throw new RobotsNullReferenceException("_robots is null");

        if (MyRobots == null)
            throw new RobotsNullReferenceException("_myRobots is null");

        if (OccupiedEnergyStations == null)
            throw new RobotsNullReferenceException("_occupiedEnergyStations is null");

        var distance = Math.Pow(Robots[myRobot].Energy, 2.0);
        EnergyStation? currentStation = null;

        foreach (var station in Map.Stations)
        {
            var nextDistance = Movement.FindDistance(station, Robots[myRobot]);

            if (nextDistance < distance && !OccupiedEnergyStations.Values.Contains(station))
            {
                currentStation = station;
                distance = nextDistance;
                
                if (nextDistance < 0.00001)
                {
                    //MyRobots[myRobot] = new CollectCommand { TargetStation = station };
                    break;
                }

            }
        }

        if (currentStation != null)
        {
            OccupiedEnergyStations.Add(myRobot, currentStation);
            return currentStation;
            // MyRobots[myRobot] = new OccupyStationCommand
            // {
            //     TargetStation = currentStation,
            //     GoToPosition = Movement.FindDestination(currentStation,
            //         Robots[myRobot], distance)
            // };
        }

        return null;
    }

    public string Author => "YushkevychAndrii";

    public string Description => "Something";
}