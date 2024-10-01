using Robot.Common;
using YushkevychAndriiRobotChallenge.Exceptions;

namespace YushkevychAndriiRobotChallenge.Commands;

public class BreedCommand : Command
{
    public int NewRobotEnergy = 100;
    public override void Process()
    {
        
        if (Algorithm.Robots?[Algorithm.RobotToMoveIndex].Energy < 200 || Algorithm.MyRobots?.Count == 100)
        {
            if (Algorithm.Robots?[Algorithm.RobotToMoveIndex].Position
                == Algorithm.OccupiedEnergyStations?[Algorithm.RobotToMoveIndex].Position)
            {
                Algorithm.TransitionTo(new CollectCommand
                {
                    TargetStation = Algorithm.OccupiedEnergyStations?[Algorithm.RobotToMoveIndex]
                });
            }

            
            var stationPosition = Algorithm.OccupiedEnergyStations?[Algorithm.RobotToMoveIndex].Position;

            if (stationPosition == null || Algorithm.Robots?[Algorithm.RobotToMoveIndex].Position != stationPosition)
            {
                if(stationPosition != null)
                    Algorithm.OccupiedEnergyStations?.Remove(Algorithm.RobotToMoveIndex);
                
                var station = Algorithm.FindStation(Algorithm.RobotToMoveIndex);

                if (station is null)
                    Algorithm.TransitionTo(new CollectCommand { TargetStation = station });

                if (station.Position == Algorithm.Robots?[Algorithm.RobotToMoveIndex].Position)
                    Algorithm.TransitionTo(new CollectCommand { TargetStation = station });
                else
                    Algorithm.TransitionTo(new OccupyStationCommand
                    {
                        TargetStation = station,
                        GoToPosition = Movement.FindDestination(station.Position,
                            Algorithm.Robots?[Algorithm.RobotToMoveIndex],
                            Movement.FindDistance(station.Position, Algorithm.Robots?[Algorithm.RobotToMoveIndex]))
                    });
            }
        }
    }

    public override RobotCommand Execute()
    {
        return new CreateNewRobotCommand(){NewRobotEnergy = NewRobotEnergy};
    }
}