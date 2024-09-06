using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Xml.Schema;
using Robot.Common;

namespace YushkevychAndriiRobotChallange;

public class YushkevychAndriiAlgorythm : IRobotAlgorithm
{
    public RobotCommand DoStep(IList<Robot.Common.Robot> robots, int robotToMoveIndex, Map map)
    {
        var manager = Manager.GetManager(robots, robotToMoveIndex, map);
        return manager.GetRobotCommand().Execute();
        // var myRobot = robots[robotToMoveIndex];
        //
        // //var targetRobot = robots.Where(robot => robot.OwnerName != this.Author).OrderByDescending(robot => robot.Energy).FirstOrDefault();
        //
        // var stations = map.Stations;
        //
        // if (stations.Any(station => station.Position == myRobot.Position)) return new CollectEnergyCommand();
        //
        // var closestStation = stations
        //     .Where(station => robots
        //         .Where(robot => robot.OwnerName == Author)
        //         .All(robot => station.Position != robot.Position))
        //     .OrderByDescending(station => FindDistance(station, myRobot))
        //     .Reverse()
        //     .FirstOrDefault();
        //
        // if (closestStation == null) return new CreateNewRobotCommand();
        //
        // double cost = FindDistance(closestStation, myRobot);
        //
        // var destination = FindDestination(closestStation, myRobot, cost);
        //
        // return new MoveCommand() { NewPosition = destination };
    }

    private Position FindDestination(EnergyStation closestStation, Robot.Common.Robot myRobot, double cost)
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

    private double FindDistance(EnergyStation station, Robot.Common.Robot myRobot)
    {
        return Math.Abs(Math.Pow(station.Position.X - myRobot.Position.X, 2.0) + Math.Pow(station.Position.Y - myRobot.Position.Y, 2.0));
    }

    public string Author => "YushkevychAndrii";

    public string Description => "Something";
}