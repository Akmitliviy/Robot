using Robot.Common;
using YushkevychAndriiRobotChallange.Exceptions;

namespace YushkevychAndriiRobotChallange.Commands;

public class CollectCommand : Command
{
    public EnergyStation TargetStation { get; set; }

    public override void Process()
    {
        var stationPosition = Algorithm.OccupiedEnergyStations?[Algorithm.RobotToMoveIndex].Position;

        if (stationPosition == null || Algorithm.Robots?[Algorithm.RobotToMoveIndex].Position != stationPosition)
        {
            if(stationPosition != null)
                Algorithm.OccupiedEnergyStations?.Remove(Algorithm.RobotToMoveIndex);
                
            var station = Algorithm.FindStation(Algorithm.RobotToMoveIndex);
            
            if (station is null)
                throw new NoAvailableStationException("Could not find station to move to in CollectCommand");
            
            if(station.Position == Algorithm.Robots?[Algorithm.RobotToMoveIndex].Position)
                Algorithm.TransitionTo(new CollectCommand{TargetStation = station});
            else
                Algorithm.TransitionTo(new OccupyStationCommand
                {
                    TargetStation = station,
                    GoToPosition = Movement.FindDestination(station.Position,
                        Algorithm.Robots?[Algorithm.RobotToMoveIndex], 
                        Movement.FindDistance(station.Position, Algorithm.Robots?[Algorithm.RobotToMoveIndex]))
                });
        }
        else if (Algorithm.Robots[Algorithm.RobotToMoveIndex].Energy >= 250 && Algorithm.MyRobots?.Count < 100)
        {
            Algorithm.TransitionTo(new BreedCommand { NewRobotEnergy = 100 });
            
        }else if (Algorithm.Robots[Algorithm.RobotToMoveIndex].Energy >= 250 && Algorithm.MyRobots?.Count == 100)
        {
            
        }
    }

    public override RobotCommand Execute()
    {
        return new CollectEnergyCommand();
    }
}