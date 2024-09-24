using System;
using Robot.Common;

namespace YushkevychAndriiRobotChallange;

public static class Movement
{
    
    public static Position FindDestination(EnergyStation closestStation, Robot.Common.Robot myRobot, double cost)
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

    public static double FindDistance(EnergyStation station, Robot.Common.Robot myRobot)
    {
        var x = Math.Pow(station.Position.X - myRobot.Position.X, 2.0);
        var reversedX = Math.Pow(100 - Math.Sqrt(Math.Pow(station.Position.X - myRobot.Position.X, 2.0)), 2.0);
       
        var y = Math.Pow(station.Position.Y - myRobot.Position.Y, 2.0);
        var reversedY = Math.Pow(100 - Math.Sqrt(Math.Pow(station.Position.Y - myRobot.Position.Y, 2.0)), 2.0);
        
        return (x < reversedX ? x : reversedX) + (y < reversedY ? y : reversedY);
    }

}