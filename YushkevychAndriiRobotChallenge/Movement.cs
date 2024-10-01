using System;
using Robot.Common;

namespace YushkevychAndriiRobotChallenge;

public struct Path
{
    public bool XIsTwisted{get;set;}
    public bool YIsTwisted{get;set;}

    public double Distance{get;set;}
}
public static class Movement
{
    
    public static Position FindDestination(Position destinationPosition, Robot.Common.Robot myRobot, Path path)
    {
        if (path.Distance > myRobot.Energy)
        {
            double fullDistance = Math.Sqrt(path.Distance);
            double currentEnergy = myRobot.Energy;

                    Position position = new Position(destinationPosition.X, destinationPosition.Y);
                    Position correction = new Position(0,0);

                    if (path.XIsTwisted)
                    {
                        var x = destinationPosition.X - myRobot.Position.X;
                        position.X = x > 0 ? x - 100 : x + 100;
                        correction.X = x > 0 ? 100 : -100;

                    }

                    if (path.YIsTwisted)
                    {
                        var y = destinationPosition.Y - myRobot.Position.Y;
                        position.Y = y > 0 ? y - 100 : y + 100;
                        correction.Y = y > 0 ? 100 : -100;

                    }
                    
            for (double currentDistance = fullDistance, k = 1; currentDistance > 0; k++, currentDistance /= k) {
                
                if(Math.Pow(currentDistance, 2.0) * k < currentEnergy)
                {
                    double ratio = currentDistance / (fullDistance - currentDistance);
                    var result = new Position(
                        Convert.ToInt32((myRobot.Position.X + ratio * position.X) / (1 + ratio)),
                        Convert.ToInt32((myRobot.Position.Y + ratio * position.Y) / (1 + ratio)));

                    result.X = result.X < 0 || result.X >= 100 ? result.X + correction.X : result.X;
                    result.Y = result.Y < 0 || result.Y >= 100 ? result.Y + correction.Y : result.Y;


                    return result;
                }
                
            }

            return myRobot.Position;
        }
        
        return destinationPosition;
    }

    public static Path FindDistance(Position destination, Robot.Common.Robot myRobot)
    {
        var x = Math.Pow(destination.X - myRobot.Position.X, 2.0);
        var reversedX = Math.Pow(100 - Math.Sqrt(x), 2.0);
       
        var y = Math.Pow(destination.Y - myRobot.Position.Y, 2.0);
        var reversedY = Math.Pow(100 - Math.Sqrt(y), 2.0);
        
        return new Path{
            Distance = (x < reversedX ? x : reversedX) + (y < reversedY ? y : reversedY),
            XIsTwisted = !(x < reversedX), 
            YIsTwisted = !(y < reversedY) 
        };
    }

}