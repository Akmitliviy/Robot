using System;
using System.Collections.Generic;
using System.Linq;
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
                throw new NoAvailableStationException("Could not find station to move to in DoStep");
            
            if(station.Position == Robots?[RobotToMoveIndex].Position)
                TransitionTo(new CollectCommand{TargetStation = station});
            else
                TransitionTo(new OccupyStationCommand
                {
                    TargetStation = station,
                    GoToPosition = Movement.FindDestination(station.Position,
                        Robots?[RobotToMoveIndex], 
                        Movement.FindDistance(station.Position, Robots?[RobotToMoveIndex]))
                });
            
        }
        GetRobotCommand()?.Process();
        var result = GetRobotCommand()?.Execute();

        return result ?? throw new InvalidOperationException();
    }

    private void SetupData(IList<Robot.Common.Robot> robots, int robotToMoveIndex, Map map)
    {
        MyRobots ??= new Dictionary<int, Command?>();
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

        var distance = Math.Pow(Convert.ToDouble(Robots[myRobot].Energy), 2.0);
        EnergyStation? currentStation = null;

        foreach (var station in Map.Stations)
        {
            var nextDistance = Movement.FindDistance(station.Position, Robots[myRobot]).Distance;

            if (nextDistance < distance && !OccupiedEnergyStations.Values.Contains(station))
            {
                currentStation = station;
                distance = nextDistance;
                
                if (nextDistance < 0.00001)
                {
                    break;
                }

            }
        }

        if (currentStation != null)
        {
            OccupiedEnergyStations.Add(myRobot, currentStation);
            return currentStation;
        }

        return null;
    }

    public string Author => "YushkevychAndrii";

    public string Description => "Something";
}